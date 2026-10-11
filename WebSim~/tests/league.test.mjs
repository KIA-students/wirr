import test from 'node:test';
import assert from 'node:assert/strict';
import {getStandings, validateLeague} from '../site/league.mjs';
import {readFileSync} from 'node:fs';

const baseline = JSON.parse(readFileSync(new URL('../../competition/scoreboard.json', import.meta.url), 'utf8'));
const clone = () => JSON.parse(JSON.stringify(baseline));
const award = points => ({points, reason:'Zatwierdzona grupowa demonstracja', approved_at:'2026-10-11T00:00:00Z', evidence:null});
const perfect = {experiment:4, reasoning:3, innovation:2, cooperation:1};
const good = {experiment:3, reasoning:2, innovation:2, cooperation:1};

test('new league is unscored and has no fictitious winner', () => {
 const state = getStandings(clone());
 assert.equal(state.completedRounds,0);
 assert.equal(state.winner,null);
 assert.deepEqual(state.groups.map(g=>g.total),[0,0]);
});
test('partial round does not award points to early team', () => {
 const board = clone();
 board.rounds[0].scores.G1 = award(perfect);
 board.rounds[0].status = 'in_progress';
 assert.equal(getStandings(board).completedRounds,0);
 assert.equal(getStandings(board).winner,null);
});
test('leader determined from fully approved rounds', () => {
 const board = clone();
 board.rounds[0].scores.G1 = award(perfect);
 board.rounds[0].scores.G2 = award(good);
 board.rounds[0].status = 'completed';
 assert.equal(getStandings(board).winner,'G1');
 assert.deepEqual(getStandings(board).groups.map(g=>g.total),[10,8]);
});
test('bad points and bad round status are rejected', () => {
 const board = clone();
 board.rounds[0].scores.G1 = award({...perfect, experiment:5});
 board.rounds[0].status = 'in_progress';
 assert.throws(()=>validateLeague(board), /Punkty poza zakresem/);
 board.rounds[0].scores.G1 = null;
 assert.throws(()=>validateLeague(board), /Niespójny status/);
});
