import {test} from 'node:test';
import assert from 'node:assert/strict';
import {score,difference,symptoms} from '../site/ssq.mjs';
test('zero and maximum responses',()=>{
 assert.equal(symptoms.length,16);
 assert.deepEqual(score(Array(16).fill(0)),{N:0,O:0,D:0,TS:0});
 assert.deepEqual(score(Array(16).fill(3)),{N:200.34,O:159.18,D:292.32,TS:179.52});
});
test('each item uses the expected subscales and contributes once to total',()=>{
 const membership=['NO','O','O','O','OD','N','N','ND','NO','D','OD','D','D','D','N','N'];
 membership.forEach((groups,i)=>{
  const v=Array(16).fill(0);v[i]=1;const s=score(v);
  assert.deepEqual(s,{N:groups.includes('N')?9.54:0,O:groups.includes('O')?7.58:0,D:groups.includes('D')?13.92:0,TS:3.74});
 });
});
test('missing, sparse, noninteger and out of range answers are rejected',()=>{
 for(const bad of [[],Array(16),Array(16).fill(null),Array(16).fill('0'),Array(16).fill(-1),Array(16).fill(4),Array(16).fill(0.5)]) assert.throws(()=>score(bad),RangeError);
});
test('before/after difference preserves improvement as negative',()=>{
 assert.deepEqual(difference(Array(16).fill(1),Array(16).fill(0)),{N:-66.78,O:-53.06,D:-97.44,TS:-59.84});
});
