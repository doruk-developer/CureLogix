using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.CouncilDTOs;
using CureLogix.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class CouncilController : Controller
    {
        private readonly ITreatmentProtocolService _protocolService;
        private readonly ICouncilVoteService _voteService;
        private readonly IDoctorService _doctorService; // <--- 1. BU SATIRI EKLE
        private readonly IMapper _mapper;

        // Constructor'ı Güncelliyoruz
        public CouncilController(
            ITreatmentProtocolService protocolService,
            ICouncilVoteService voteService,
            IDoctorService doctorService, // <--- 2. BURAYA PARAMETRE OLARAK EKLE
            IMapper mapper)
        {
            _protocolService = protocolService;
            _voteService = voteService;
            _doctorService = doctorService; // <--- 3. BURADA ATAMA YAP
            _mapper = mapper;
        }

        // 1. Bekleyen Protokolleri Listele
        public IActionResult Index()
        {
            // 1. Bekleyen Protokolleri Çek
            var pendingProtocols = _protocolService.TGetList().Where(x => x.Status == 0).ToList();

            // 2. DTO Listesi Hazırla
            var dtoList = new List<CouncilListDto>();

            foreach (var item in pendingProtocols)
            {
                // Doktorun ismini bul
                var doctor = _doctorService.TGetById(item.DoctorId);

                dtoList.Add(new CouncilListDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    CreatedDate = item.CreatedDate,
                    DoctorName = doctor != null ? doctor.FullName : "Bilinmiyor" // İsim Ataması
                });
            }

            return View(dtoList);
        }

        // 2. Detay ve Oylama Sayfası (GET)
        [HttpGet]
        public IActionResult Review(int id)
        {
            // Protokolü bul
            var protocol = _protocolService.TGetById(id);

            // Eğer ilişkili veriler (Disease, Doctor, Medicines) null gelirse, 
            // Entity Framework "Lazy Loading" veya "Include" eksikliğinden olabilir.
            // Şimdilik DTO'ya çevirip gönderelim.

            var detailDto = _mapper.Map<ProtocolDetailDto>(protocol);

            // View'a hem detayları hem de boş bir oy formunu göndermek için Tuple veya ViewModel kullanabiliriz.
            // Kolaylık olsun diye DTO'yu ViewBag ile, Oy formunu Model ile taşıyalım (veya tam tersi).

            ViewBag.ProtocolDetail = detailDto;

            return View(new VoteOperationDto { ProtocolId = id });
        }

        // 3. Oyu Kaydet (POST)
        [HttpPost]
        [AuditLog("Konsey Oylaması Yapıldı")]
        public IActionResult Review(VoteOperationDto p)
        {
            p.RefereeDoctorId = 5; // Simülasyon: Şimdilik "Ord. Prof. Hikmet Vural" (ID=5) oy veriyor varsayalım.
                                   // İleride Login olan kullanıcı ID'si gelecek.

            var voteEntity = _mapper.Map<CouncilVote>(p);
            voteEntity.VoteDate = DateTime.Now;

            _voteService.TAdd(voteEntity);

            // STATE MACHINE (Basit Versiyon):
            // Eğer "Kabul" verildiyse Protokolü Onayla, "Red" ise Reddet.
            var protocol = _protocolService.TGetById(p.ProtocolId);
            if (p.VoteResult)
            {
                protocol.Status = 1; // Onaylandı
            }
            else
            {
                protocol.Status = 2; // Reddedildi
            }
            _protocolService.TUpdate(protocol);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChatRoom()
        {
            // Şimdilik kullanıcı adı simülasyonu yapıyoruz
            // Gerçekte Identity'den gelir. Burada rastgele bir Doktor adı atayalım.
            var randomDoctors = new List<string> { "Prof. Dr. Hikmet Vural", "Doç. Dr. Selim Arca", "Dr. Ayşe Yılmaz", "Uzm. Dr. Kemal Sayar" };
            Random rnd = new Random();
            ViewBag.UserName = randomDoctors[rnd.Next(randomDoctors.Count)]; // Rastgele bir isim seç

            return View();
        }
    }
}