/* Local-only team-name generator: no account, network or personal data. */
export const ADJECTIVES = Object.freeze([
  'Kwantowe', 'Pikselowe', 'Cybernetyczne', 'Rekurencyjne',
  'Wirtualne', 'Neonowe', 'Rozbrykane', 'Zrobotyzowane',
  'Bezkablowe', 'Samoświadome', 'Binarne', 'Lewitujące',
  'Algorytmiczne', 'Równoległe', 'Nadprzewodzące', 'Zhakowane',
  'Turbo', 'Laserowe', 'Holograficzne', 'Mechaniczne'
]);
export const NOUNS = Object.freeze([
  'Kapibary', 'Pierogi', 'Dziobaki', 'Wombaty',
  'Wektory', 'Bajty', 'Roboty', 'Drony', 'Jeże',
  'Kaktusy', 'Żółwie', 'Śrubokręty', 'Serwomechanizmy',
  'Pingwiny', 'Transformatory', 'Gogle', 'Czipy',
  'Piksele', 'Czajniki', 'Ryśki'
]);
export function suggestNames(count = 3, random = Math.random) {
  const maximum = ADJECTIVES.length * NOUNS.length;
  const limit = Math.min(Math.max(0, Math.floor(count)), 8);
  const used = new Set(), results = [];
  for (let i=0; i<limit; i++) {
    // The draw is clamped; when a custom RNG repeats, fall back to the next
    // combination so exactly three different proposals are still returned.
    let index = Math.min(maximum - 1, Math.max(0, Math.floor(random() * maximum)));
    while (used.has(index)) index = (index + 1) % maximum;
    used.add(index);
    results.push(ADJECTIVES[Math.floor(index / NOUNS.length)] + ' ' + NOUNS[index % NOUNS.length]);
  }
  return results;
}
