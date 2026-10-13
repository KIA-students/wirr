#!/usr/bin/env python3
"""Export accepted WiRR reports from private Supabase queue to course GitHub repository."""
import base64
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request

BASE = "https://ndebunrsdtulflzpemwd.supabase.co"
TARGET = "KIA-students/wirr"
API = "https://api.github.com"
SUPABASE_KEY = os.environ.get("WIRR_SUPABASE_SERVICE_ROLE_KEY", "")
GITHUB_KEY = os.environ.get("KIA_STUDENTS_SYNC_TOKEN", "")

def request(method, url, headers, data=None):
    payload = None if data is None else json.dumps(data, ensure_ascii=False).encode("utf-8")
    req = urllib.request.Request(url, data=payload, headers=headers, method=method)
    with urllib.request.urlopen(req, timeout=25) as response:
        content = response.read()
        return json.loads(content) if content else {}

def auth_supabase():
    return {"apikey": SUPABASE_KEY, "Authorization": "Bearer " + SUPABASE_KEY, "Content-Type": "application/json"}

def auth_github():
    return {"Authorization": "Bearer " + GITHUB_KEY, "Accept": "application/vnd.github+json", "Content-Type": "application/json", "X-GitHub-Api-Version": "2022-11-28"}

def main():
    if not SUPABASE_KEY or not GITHUB_KEY:
        print("Required WIRR_SUPABASE_SERVICE_ROLE_KEY and KIA_STUDENTS_SYNC_TOKEN secrets are missing.", file=sys.stderr)
        return 2
    url = BASE + "/rest/v1/wirr_public_intake?select=submission_id,group_code,lab_no,report,status,github_path&status=eq.pending&order=received_at.asc&limit=30"
    rows = request("GET", url, auth_supabase())
    for row in rows:
        submission_id, group, lab = row["submission_id"], row["group_code"], row["lab_no"]
        if group not in ("G1", "G2") or not isinstance(lab, int) or not 1 <= lab <= 8:
            print("Invalid group or laboratory:", submission_id, file=sys.stderr)
            continue
        path = f"student-work/{group}/lab{lab:02d}-{submission_id}.json"
        # Single source of truth: never publish if the group/lab differs from the validated path.
        if row["github_path"] != path:
            print("Path mismatch:", submission_id, file=sys.stderr)
            continue
        raw = (json.dumps(row["report"], ensure_ascii=False, indent=2) + "\n").encode("utf-8")
        destination = API + "/repos/" + TARGET + "/contents/" + urllib.parse.quote(path, safe="/")
        name = "wirr-report/" + submission_id
        try:
            # Create a separate branch and a reviewable PR, never commit student reports to main directly.
            base = request("GET", API + "/repos/" + TARGET + "/git/ref/heads/main", auth_github())
            try:
                request("POST", API + "/repos/" + TARGET + "/git/refs", auth_github(),
                        {"ref":"refs/heads/" + name, "sha":base["object"]["sha"]})
            except urllib.error.HTTPError as branch_error:
                if branch_error.code != 422: raise
            try:
                old = request("GET", destination + "?ref=" + urllib.parse.quote(name), auth_github())
                existing_sha = old.get("sha")
            except urllib.error.HTTPError as missing:
                if missing.code != 404: raise
                existing_sha = None
            body = {"message":f"student-work: WiRR {group} Lab {lab:02d} report", "content":base64.b64encode(raw).decode(), "branch":name}
            if existing_sha: body["sha"] = existing_sha
            commit = request("PUT", destination, auth_github(), body)
            sha = commit.get("commit",{}).get("sha")
            if not sha: raise RuntimeError("GitHub did not confirm report commit")
            existing_prs = request("GET", API + "/repos/" + TARGET + "/pulls?head=KIA-students:" + urllib.parse.quote(name) + "&state=open", auth_github())
            if existing_prs:
                pr = existing_prs[0]
            else:
                pr = request("POST",API + "/repos/" + TARGET + "/pulls",auth_github(),{
                    "title":f"WiRR {group} Lab {lab:02d}: zgłoszenie raportu",
                    "head":name,"base":"main",
                    "body":"Automatyczne zgłoszenie z Unity. Sprawdź zawartość i uprawnienie do jej publikacji przed scaleniem."
                })
            receipt = {"status":"reviewed", "github_path":path, "github_commit_sha":sha, "github_pr_url":pr["html_url"], "last_error":None}
            request("PATCH",BASE+"/rest/v1/wirr_public_intake?submission_id=eq."+submission_id,
                    {**auth_supabase(),"Prefer":"return=minimal"},receipt)
            print(f"GitHub PR confirmed: {pr['html_url']}")
        except Exception as exc:
            print(f"Failed to publish {submission_id}: {exc}", file=sys.stderr)
            # Preserve pending for safe retry; never imply delivery without GitHub SHA.
            return 1
    print("Queue scan complete:",len(rows),"rows")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
