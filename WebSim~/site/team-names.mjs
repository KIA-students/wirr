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
  const limit = Math.min(Math.max(0, Math.floor(count)), 8);
  const result = new Set();
  let attempts = 0;
  while (result.size < limit && attempts < 120) {
    const a = ADJECTIVES[Math.floor(random() * ADJECTIVES.length) % ADJECTIVES.length];
    const b = NOUNS[Math.floor(random() * NOUNS.length) % NOUNS.length];
    result.add(a + ' ' + b);
    attempts++;
  }
  return [...result];
}
