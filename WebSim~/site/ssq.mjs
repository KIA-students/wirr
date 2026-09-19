// Item order and original SSQ weights: Kennedy et al. (1993), doi:10.1207/s15327108ijap0303_3.
export const symptoms = [
 ['Ogólny dyskomfort','General discomfort'], ['Zmęczenie','Fatigue'],
 ['Ból głowy','Headache'], ['Zmęczenie oczu','Eyestrain'],
 ['Trudność z ustawieniem ostrości wzroku','Difficulty focusing'],
 ['Zwiększone wydzielanie śliny','Increased salivation'], ['Pocenie się','Sweating'],
 ['Nudności','Nausea'], ['Trudność z koncentracją','Difficulty concentrating'],
 ['Uczucie pełności w głowie','Fullness of head'], ['Niewyraźne widzenie','Blurred vision'],
 ['Zawroty głowy przy otwartych oczach','Dizzy (eyes open)'],
 ['Zawroty głowy przy zamkniętych oczach','Dizzy (eyes closed)'],
 ['Wrażenie wirowania','Vertigo'], ['Odczuwanie żołądka / dyskomfort w żołądku','Stomach awareness'],
 ['Odbijanie','Burping']
];
export function score(values) {
 if (!Array.isArray(values) || values.length !== 16 || Array.from(values).some(v => !Number.isInteger(v) || v < 0 || v > 3)) {
  throw new RangeError('Wymagane jest 16 odpowiedzi całkowitych w zakresie 0–3.');
 }
 const sum = items => items.reduce((s, i) => s + values[i - 1], 0);
 const round = n => Math.round(n * 100) / 100;
 return { N: round(sum([1,6,7,8,9,15,16]) * 9.54), O: round(sum([1,2,3,4,5,9,11]) * 7.58),
  D: round(sum([5,8,10,11,12,13,14]) * 13.92), TS: round(values.reduce((a,b) => a+b,0) * 3.74) };
}
export function difference(pre, post) {
 const a = score(pre), b = score(post);
 return Object.fromEntries(Object.keys(a).map(k => [k, Math.round((b[k]-a[k])*100)/100]));
}
