#if UNITY_EDITOR
// WiRR 1.1.3: direct KIA submission with local retry queue.
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
public sealed class WiRRKiaReports : EditorWindow {
 const string Api="https://ndebunrsdtulflzpemwd.supabase.co";
 const string Key="sb_publishable_00hCok_8Bm3nteWusHg2Eg_5fFwDsps";
 static readonly HttpClient Http=new HttpClient{Timeout=TimeSpan.FromSeconds(15)};
 string email="",password="",jwt="",uid="",course="",message="Zaloguj się na konto KIA."; bool busy;
 [Serializable] class Login { public string email,password; }
 [Serializable] class Auth { public string access_token; public Account user; }
 [Serializable] class Account { public string id; }
 [Serializable] class Enrollment { public string course_id; }
 [Serializable] class Course { public string id,code; }
 [Serializable] class List<T> { public T[] items; }
 [Serializable] class Item { public string id,owner,course,report; public int lab; }
 static T[] Array<T>(string value) { var x=JsonUtility.FromJson<List<T>>("{\"items\":"+value+"}"); return x.items??System.Array.Empty<T>(); }
 static string Folder { get { var p=Path.Combine(Directory.GetParent(Application.dataPath).FullName,"UserSettings","WiRRKiaQueue");Directory.CreateDirectory(p);return p; } }
 [MenuItem("WiRR/Student/Wyślij raport do KIA")]
 static void ShowWindow() => GetWindow<WiRRKiaReports>("WiRR / KIA");
 void OnGUI() {
  EditorGUILayout.HelpBox(message,MessageType.Info);
  if(string.IsNullOrEmpty(jwt)){
   email=EditorGUILayout.TextField("Adres konta KIA",email);
   password=EditorGUILayout.PasswordField("Hasło KIA",password);
   using(new EditorGUI.DisabledScope(busy||string.IsNullOrEmpty(email)||string.IsNullOrEmpty(password)))
    if(GUILayout.Button("Zaloguj")) _=SignIn();
  }else{
   EditorGUILayout.LabelField("Kurs:",string.IsNullOrEmpty(course)?"Nie przypisano do WiRR":course);
   using(new EditorGUI.DisabledScope(busy||string.IsNullOrEmpty(course))){
    if(GUILayout.Button("Wyślij raport JSON")){
     var path=EditorUtility.OpenFilePanel("Raport WiRR",Directory.GetParent(Application.dataPath).FullName,"json");
     if(!string.IsNullOrEmpty(path)) _=AddAndSend(path);
    }
    if(GUILayout.Button("Ponów wysyłkę")) _=Flush();
   }
   if(GUILayout.Button("Wyloguj")){jwt="";uid="";password="";course="";}
  }
 }
 static HttpRequestMessage Req(HttpMethod method,string path,string token=""){
  var r=new HttpRequestMessage(method,Api+path);
  r.Headers.TryAddWithoutValidation("apikey",Key);
  if(token!="")r.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
  return r;
 }
 static async Task<string> Do(HttpRequestMessage r){
  using(r)using(var response=await Http.SendAsync(r)){
   var body=await response.Content.ReadAsStringAsync();
   if(!response.IsSuccessStatusCode)throw new Exception("HTTP "+(int)response.StatusCode+": "+body);
   return body;
  }
 }
 async Task SignIn(){
  busy=true;
  try{
   var r=Req(HttpMethod.Post,"/auth/v1/token?grant_type=password");
   r.Content=new StringContent(JsonUtility.ToJson(new Login{email=email,password=password}),Encoding.UTF8,"application/json");
   var result=JsonUtility.FromJson<Auth>(await Do(r));
   if(result==null||result.user==null||string.IsNullOrEmpty(result.access_token))throw new Exception("Nie otrzymano sesji.");
   jwt=result.access_token;uid=result.user.id;password="";
   var enrolled=Array<Enrollment>(await Do(Req(HttpMethod.Get,"/rest/v1/enrollments?select=course_id&active=eq.true",jwt)));
   foreach(var e in enrolled){
    var courses=Array<Course>(await Do(Req(HttpMethod.Get,"/rest/v1/courses?select=id,code&id=eq."+Uri.EscapeDataString(e.course_id),jwt)));
    foreach(var c in courses)if(c.code.ToUpperInvariant()=="WIRR"){course=c.id;break;}
    if(course!="")break;
   }
   message=course==""?"Nie znaleziono aktywnego zapisu na kurs WIRR.":"Zalogowano. Raport można wysłać jednym przyciskiem.";
  }catch(Exception ex){jwt="";message="Błąd logowania: "+ex.Message;}
  finally{busy=false;Repaint();}
  if(course!="")await Flush();
 }
 async Task AddAndSend(string path){
  try{
   var name=Path.GetFileNameWithoutExtension(path).ToLowerInvariant();int lab=0;
   for(int i=1;i<=8;i++)if(name.Contains("lab"+i.ToString("00"))){lab=i;break;}
   if(lab==0)throw new Exception("Nazwa pliku musi zawierać Lab01 ... Lab08.");
   var data=File.ReadAllText(path,Encoding.UTF8).Trim();
   if(!data.StartsWith("{")||!data.EndsWith("}"))throw new Exception("Raport nie jest obiektem JSON.");
   var item=new Item{id=Guid.NewGuid().ToString(),owner=uid,course=course,lab=lab,report=data};
   File.WriteAllText(Path.Combine(Folder,item.id+".json"),JsonUtility.ToJson(item),Encoding.UTF8);
   await Flush();
  }catch(Exception ex){message="Raport pozostaje lokalnie. "+ex.Message;Repaint();}
 }
 async Task Flush(){
  if(busy)return;busy=true;int count=0;
  try{
   foreach(var file in Directory.GetFiles(Folder,"*.json")){
    var item=JsonUtility.FromJson<Item>(File.ReadAllText(file,Encoding.UTF8));
    if(item.owner!=uid||item.course!=course)continue;
    var payload="{\"client_submission_id\":\""+item.id+"\",\"course_id\":\""+item.course+"\",\"student_id\":\""+item.owner+"\",\"lab_no\":"+item.lab+",\"report\":"+item.report+"}";
    var r=Req(HttpMethod.Post,"/rest/v1/wirr_submissions?on_conflict=client_submission_id",jwt);
    r.Headers.TryAddWithoutValidation("Prefer","resolution=ignore-duplicates,return=representation");
    r.Content=new StringContent(payload,Encoding.UTF8,"application/json");
    var answer=await Do(r);
    // Empty array is expected on retry; uniqueness means the same submission was already accepted.
    File.Delete(file);count++;
   }
   message="Serwer KIA potwierdził przyjęcie "+count+" zgłoszeń.";
  }catch(Exception ex){message="Brak potwierdzenia serwera. Zachowano kolejkę: "+ex.Message;}
  finally{busy=false;Repaint();}
 }
}
#endif
