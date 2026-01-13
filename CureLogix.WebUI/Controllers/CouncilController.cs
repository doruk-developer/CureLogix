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
        private readonly IMapper _mapper;

        public CouncilController(ITreatmentProtocolService protocolService, ICouncilVoteService voteService, IMapper mapper)
        {
            _protocolService = protocolService;
            _voteService = voteService;
            _mapper = mapper;
        }

        // 1. Bekleyen Protokolleri Listele
        public IActionResult Index()
        {
            // Status == 0 (Bekleyenler)
            // NOT: Burada normalde "Include" ile ilişkili tabloları çekmek gerekir.
            // Şimdilik bütün listeyi çekip filtreliyoruz (Performans için ileride Repository güncellenmeli)
            var allProtocols = _protocolService.TGetList();
            var pendingProtocols = allProtocols.Where(x => x.Status == 0).ToList();

            // Listeleme için basit bir DTO veya direkt Entity kullanabiliriz şimdilik
            return View(pendingProtocols);
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