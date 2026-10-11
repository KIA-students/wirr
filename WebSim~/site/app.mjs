import { symptoms, score, difference } from './ssq.mjs';
import { renderLeague } from './league.mjs';
import { suggestNames } from './team-names.mjs';
const $ = id => document.getElementById(id);
const links = [...document.querySelectorAll('nav a')];
function navigate() {
 const id = ['unity','websim','ssq','liga'].includes(location.hash.slice(1)) ? location.hash.slice(1) : 'unity';
 for (const link of links) {
  const selected = link.hash === `#${id}`;
  if (selected) link.setAttribute('aria-current','page'); else link.removeAttribute('aria-current');
  $(link.hash.slice(1)).hidden = !selected;
 }
 if (id === 'liga') renderLeague($('league-results'));
}
window.addEventListener('hashchange', navigate);
$('league-refresh').addEventListener('click', () => renderLeague($('league-results')));
navigate();

const groupNameDraft = document.getElementById('team-name-own');
const generatorOutput = document.getElementById('team-name-proposals');
const generatorStatus = document.getElementById('team-name-feedback');
document.getElementById('team-name-generate').addEventListener('click', () => {
 generatorOutput.replaceChildren();
 for (const proposed of suggestNames(3)) {
  const button = document.createElement('button');
  button.type = 'button';
  button.textContent = proposed;
  button.addEventListener('click', () => {
   groupNameDraft.value = proposed;
   generatorStatus.textContent = 'Wybrana propozycja: ' + proposed + '. Pokaż ją prowadzącemu.';
  });
  generatorOutput.append(button);
 }
});
document.getElementById('team-name-copy').addEventListener('click', async () => {
 const name = groupNameDraft.value.trim().replace(/\s+/g,' ');
 if (name.length < 3 || name.length > 48 || !/^[\p{L}\p{N} -]+$/u.test(name)) {
  generatorStatus.textContent = 'Podaj nazwę o długości 3–48 znaków (litery, cyfry i spacje).';
  return;
 }
 const group = document.getElementById('team-name-group').value;
 const proposal = group + ': ' + name;
 try {
  await navigator.clipboard.writeText(proposal);
  generatorStatus.textContent = 'Skopiowano. Wyślij propozycję prowadzącemu lub pokaż na zajęciach.';
 } catch {
  generatorStatus.textContent = 'Propozycja do przekazania prowadzącemu: ' + proposal;
 }
});
const answers = { pre: Array(16).fill(null), post: Array(16).fill(null) };
const times = {pre: null, post: null};
let phase = 'pre';
const complete = p => answers[p].every(v => v !== null);
const format = n => n.toLocaleString('pl-PL', {minimumFractionDigits:2,maximumFractionDigits:2});
function renderQuestions() {
 $('questions').replaceChildren();
 symptoms.forEach(([pl,en],i) => {
  const field = document.createElement('fieldset');
  const legend = document.createElement('legend');
  legend.textContent = `${i+1}. ${pl}`;
  const small = document.createElement('small'); small.textContent = en; legend.append(small); field.append(legend);
  const choices = document.createElement('div'); choices.className = 'choices';
  ['Brak','Niewielkie','Umiarkowane','Silne'].forEach((label,v) => {
   const item = document.createElement('label');
   const radio = document.createElement('input'); radio.type='radio'; radio.name=`q${i}`; radio.value=v; radio.required=true;
   radio.checked = answers[phase][i] === v;
   radio.addEventListener('change', () => { answers[phase][i]=v; times[phase]=new Date().toISOString(); update(); });
   item.append(radio, ` ${v} · ${label}`); choices.append(item);
  });
  field.append(choices); $('questions').append(field);
 });
 update();
}
function update() {
 const count = answers[phase].filter(v => v !== null).length;
 $('progress').textContent = `${phase === 'pre' ? 'Przed' : 'Po'} ekspozycji: ${count}/16 odpowiedzi. ${count < 16 ? 'Uzupełnij wszystkie objawy.' : 'Pomiar kompletny.'}`;
 $('export').disabled = !complete('pre') && !complete('post');
 const available = ['pre','post'].filter(complete);
 $('results').replaceChildren();
 if (!available.length) return;
 const title = document.createElement('h3'); title.textContent='Wyniki SSQ'; $('results').append(title);
 const wrap=document.createElement('div'); wrap.className='table-wrap';
 const table=document.createElement('table');
 const caption=document.createElement('caption'); caption.textContent='Wyniki ważone; Δ = po − przed'; table.append(caption);
 const head=document.createElement('thead'); const hr=document.createElement('tr');
 ['Pomiar','N','O','D','TS'].forEach(t => { const th=document.createElement('th'); th.scope='col'; th.textContent=t; hr.append(th); }); head.append(hr); table.append(head);
 const body=document.createElement('tbody');
 const rows=available.map(p=>[p==='pre'?'Przed':'Po',score(answers[p])]);
 if (available.length===2) rows.push(['Δ',difference(answers.pre,answers.post)]);
 for (const [label,values] of rows) {
  const tr=document.createElement('tr'); const th=document.createElement('th'); th.scope='row'; th.textContent=label; tr.append(th);
  for(const k of ['N','O','D','TS']) { const td=document.createElement('td'); td.textContent=format(values[k]); tr.append(td); } body.append(tr);
 }
 table.append(body); wrap.append(table); $('results').append(wrap);
}
$('phase').addEventListener('change', event => { phase=event.target.value; renderQuestions(); });
$('ssq-form').addEventListener('submit', event => { event.preventDefault(); update(); $('results').scrollIntoView({behavior:'smooth',block:'center'}); });
$('reset').addEventListener('click', () => {
 if (!window.confirm('Usunąć odpowiedzi z obu pomiarów? Pobierz wyniki, jeśli chcesz je zachować.')) return;
 answers.pre.fill(null); answers.post.fill(null); times.pre=null; times.post=null; phase='pre'; $('phase').value='pre'; renderQuestions();
});
$('export').addEventListener('click', () => {
 const data={schema:'wirr-ssq/1.0', exportedAt:new Date().toISOString(), translation:'Polish working translation; not validated',
  source:'https://doi.org/10.1207/s15327108ijap0303_3', scoring:'N*9.54; O*7.58; D*13.92; sum of 16 items counted once *3.74',
  items:symptoms.map(([pl,en],i)=>({number:i+1,pl,en})), measurements:{}};
 for(const p of ['pre','post']) data.measurements[p]={complete:complete(p),lastAnsweredAt:times[p],answers:[...answers[p]],scores:complete(p)?score(answers[p]):null};
 data.delta=complete('pre')&&complete('post')?difference(answers.pre,answers.post):null;
 const url=URL.createObjectURL(new Blob([JSON.stringify(data,null,2)],{type:'application/json'}));
 const a=document.createElement('a'); a.href=url; a.download='wirr-ssq.json'; a.click(); setTimeout(()=>URL.revokeObjectURL(url),1000);
});
renderQuestions();
