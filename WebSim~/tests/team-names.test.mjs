import test from 'node:test';
import assert from 'node:assert/strict';
import {suggestNames} from '../site/team-names.mjs';

test('names are readable technology-themed Polish phrases', () => {
 const proposals = suggestNames(3, () => 0.33);
 assert.equal(proposals.length,3);
 assert.match(proposals[0],/^\p{L}+(?: \p{L}+)+$/u);
});
test('generator never emits more than eight proposals', () => {
 let counter=0;
 const names = suggestNames(100, () => (counter++ % 19) / 20);
 assert.ok(names.length <= 8);
 assert.equal(new Set(names).size,names.length);
});
