using MeetingOrganizer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeetingOrganizer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Meeting> meetings = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");

                //Http Get = Server çalışıyor mu kontrol et
                var responceTask = client.GetAsync("/meeting");
                responceTask.Wait();//gönderdiğim cevap gelene kadar bekle

                var result = responceTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //Datanın içindeki json ı okutuyoruz
                    var readTask = result.Content.ReadAsAsync<IEnumerable<Meeting>>();
                    readTask.Wait();

                    meetings = readTask.Result;
                }
                else
                {
                    meetings = Enumerable.Empty<Meeting>();
                    ModelState.AddModelError(string.Empty, "Sunucu Bulunamadı veya Kayıt yok.");
                }
            }
            return View(meetings);
        }
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(Meeting meeting)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var postTask = client.PostAsJsonAsync<Meeting> ("/meeting", meeting);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Hata oluştu");
                    return View(meeting);
                }
            }
        }
        public IActionResult Details(int id)
        {
            Meeting meeting = null;
            using(var client = new HttpClient ())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var getTask = client.GetAsync("/meeting/" + id.ToString());
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJsonTask = result.Content.ReadAsAsync<Meeting>();
                    readJsonTask.Wait();
                    meeting = readJsonTask.Result;

                }
                else
                {
                    meeting = new Meeting();
                    ModelState.AddModelError(string.Empty, "Meeting Bulunamadı");
                }
            }
            return View(meeting);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Meeting meeting = null;
            using (var client = new HttpClient())
            {
                 client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var getTask = client.GetAsync("/meeting/" + id.ToString());
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJsonTask = result.Content.ReadAsAsync<Meeting>();
                    readJsonTask.Wait();
                    meeting = readJsonTask.Result;//Dönen değeri derginin içine atadık.
                }
                else
                {
                    meeting = new Meeting();
                    ModelState.AddModelError(string.Empty, "Meeting Bulunamadı. ");
                }
            }
            return View(meeting);

        }
        [HttpPost]
        public IActionResult Edit(int id, Meeting meeting)
        {
            if (id != meeting.id)
            {
                ModelState.AddModelError(string.Empty, "Talep edilen dergi ile değiştirilen dergi aynı değil. ");
                //result.ReasonPhrase => hangi hata mesajı geldiğini gösterdik
                return View(meeting);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var putTask = client.PutAsJsonAsync<Meeting>("/meeting/" + id.ToString(), meeting);
                putTask.Wait();
                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Hata oluştu" + result.ReasonPhrase);
                    //result.ReasonPhrase => hangi hata mesajı geldiğini gösterdik
                    return View(meeting);
                }
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {

            Meeting meeting = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var getTask = client.DeleteAsync("/meeting/" + id.ToString());
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJsonTask = result.Content.ReadAsAsync<Meeting>();
                    readJsonTask.Wait();
                    meeting = readJsonTask.Result;//Dönen değeri derginin içine atadık.
                }
                else
                {
                    meeting = new Meeting();
                    ModelState.AddModelError(string.Empty, "Dergi Bulunamadı. ");
                }
            }
            return View(meeting);
        }
        [HttpPost]
        public IActionResult Delete(int id, Meeting meeting)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
                var deleteTask = client.GetAsync("/meeting/" + id.ToString());
                deleteTask.Wait();

                if (deleteTask.Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(deleteTask.Result.StatusCode.ToString(),
                        deleteTask.Result.ReasonPhrase);
                    return View(id);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
