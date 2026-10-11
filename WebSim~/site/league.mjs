/* Read-only public league; no score updates or identity collection in browser. */
const LIMITS = { experiment: 4, reasoning: 3, innovation: 2, cooperation: 1 };

export function validateLeague(board) {
  if (!board || board.schema_version !== 'wirr-team-league/1') throw new Error('Nieznany format');
  if (!Array.isArray(board.groups) || board.groups.map(x=>x.id).join(',') !== 'G1,G2') throw new Error('Grupy');
  if (!Array.isArray(board.rounds) || board.rounds.map(x=>x.id).join(',') !== '1,2,3,4') throw new Error('Rundy');
  for (const round of board.rounds) {
    if (!round.scores) throw new Error('Brak punktów');
    let approved = 0;
    for (const id of ['G1', 'G2']) {
      const entry = round.scores[id];
      if (entry === null) continue;
      if (!entry || !entry.points || typeof entry.reason !== 'string') throw new Error('Wynik');
      if (Object.keys(entry.points).sort().join(',') !== Object.keys(LIMITS).sort().join(',')) throw new Error('Kategorie');
      for (const [key, maximum] of Object.entries(LIMITS)) {
        const value = entry.points[key];
        if (!Number.isInteger(value) || value < 0 || value > maximum) throw new Error('Punkty poza zakresem');
      }
      approved++;
    }
    const status = approved === 2 ? 'completed' : approved === 1 ? 'in_progress' : 'pending';
    if (round.status !== status) throw new Error('Niespójny status rundy');
  }
  if (!board.sidequests || !Array.isArray(board.sidequests.G1) || !Array.isArray(board.sidequests.G2)) throw new Error('Brak danych bonusowych');
  for (const id of ['G1','G2']) {
    const items=board.sidequests[id];
    if (items.length>4 || new Set(items.map(x=>x.task)).size!==items.length) throw new Error('Powtórzone zadania');
    for (const item of items) if (!Number.isInteger(item.points) || item.points<1 || item.points>2) throw new Error('Błędne bonusy');
    if(items.reduce((n,x)=>n+x.points,0)>8) throw new Error('Przekroczono bonus');
  }
  return board;
}

export function getStandings(board) {
  validateLeague(board);
  const done = board.rounds.filter(round => round.status === 'completed');
  const groups = board.groups.map(group => ({
    id: group.id, name: group.label, completed: done.length,
    lab: done.reduce((sum, round) => sum +
      Object.values(round.scores[group.id].points).reduce((a,b) => a+b, 0), 0),
    bonus: board.sidequests[group.id].reduce((n,item)=>n+item.points,0),
  }));
  for(const group of groups) group.total=group.lab+group.bonus;
  const [a,b] = groups;
  return {
    groups, completedRounds: done.length, isFinal: done.length === 4,
    winner: (done.length === 0 && a.bonus === 0 && b.bonus === 0) || a.total === b.total ? null : a.total > b.total ? 'G1' : 'G2',
  };
}

function el(tag, text, css = '') {
  const node = document.createElement(tag);
  if (text !== undefined) node.textContent = String(text);
  if (css) node.className = css;
  return node;
}

function render(board, target) {
  const summary = getStandings(board);
  target.replaceChildren();
  const headline = summary.completedRounds === 0 ? 'Jeszcze bez rozstrzygniętych rund' :
    summary.winner === null ? 'Remis po zatwierdzonych rundach' :
    (summary.isFinal ? 'Wygrywa: ' : 'Prowadzi: ') +
      board.groups.find(group => group.id === summary.winner).label;
  target.append(el('h3', headline));
  target.append(el('p',
    'Zatwierdzone rundy: ' + summary.completedRounds + '/4; maks. bonus +8. ' +
    'Liczą się tylko rundy ocenione dla obu grup. ' +
    (board.updated_at ? 'Ostatnia aktualizacja: ' + new Date(board.updated_at).toLocaleString('pl-PL') + '.' : 'Punktów jeszcze nie przyznano.'),
    'league-muted'));
  const grid = el('div', undefined, 'league-grid');
  for (const group of summary.groups) {
    const tile = el('section', undefined, 'league-tile');
    tile.append(el('h3', group.name), el('strong', group.total + ' / 48 pkt', 'league-number'));
    tile.append(el('p', 'Bloki: ' + group.completed + '/4 | Misje dodatkowe: +' + group.bonus));
    grid.append(tile);
  }
  target.append(grid);
  const wrap = el('div', undefined, 'table-wrap');
  const table = el('table');
  table.append(el('caption', 'Brak wyniku oznacza oczekiwanie na ocenę, nie zero'));
  const head = el('thead'), row = el('tr');
  ['Blok', 'Grupa 1', 'Grupa 2', 'Status'].forEach(name => row.append(el('th', name)));
  head.append(row); table.append(head);
  const body = el('tbody');
  for (const round of board.rounds) {
    const row = el('tr');
    row.append(el('th', round.id + '. ' + round.title));
    for (const id of ['G1', 'G2']) {
      const entry = round.scores[id];
      const value = entry === null ? 'Oczekuje' :
        Object.values(entry.points).reduce((a,b) => a+b,0) + ' / 10';
      row.append(el('td', value));
    }
    row.append(el('td', round.status === 'completed' ? 'Zatwierdzona' :
      round.status === 'pending' ? 'Nie rozpoczęto' : 'W trakcie oceny'));
    body.append(row);
  }
  table.append(body); wrap.append(table); target.append(wrap);
  target.append(el('p', 'Punkty w blokach i opcjonalne misje dodatkowe zatwierdza prowadzący. Brak udziału w dodatkowych misjach nie wpływa na indywidualną ocenę.'));
  const p = el('p');
  const a = el('a', 'Regulamin i plik wyników na GitHubie');
  a.href = 'https://github.com/KIA-students/wirr/tree/main/competition';
  a.rel = 'noopener noreferrer'; p.append(a); target.append(p);
}

export async function renderLeague(target) {
  if (!target) return;
  target.replaceChildren(el('p', 'Wczytywanie rankingu…'));
  try {
    const response = await fetch('./competition/scoreboard.json', {cache:'no-store'});
    if (!response.ok) throw new Error('HTTP ' + response.status);
    render(validateLeague(await response.json()), target);
  } catch {
    target.replaceChildren(el('p', 'Ranking nie jest jeszcze opublikowany lub wystąpił błąd odczytu. Zobacz repozytorium kursu.'));
  }
}
