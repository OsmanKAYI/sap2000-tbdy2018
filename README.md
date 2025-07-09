# SAP2000 TBDY-2018 Model Üretici

Modern bir Windows Forms uygulaması olan bu araç, TS500 ve TBDY-2018 Türk bina yönetmeliklerine uygun, parametrik SAP2000 yapı modellerinin hızlı ve doğru bir şekilde oluşturulması için tasarlanmıştır. Mühendislerin malzeme, kesit, kat bilgileri, eleman yerleşimleri ve kritik deprem parametrelerini kolayca tanımlamalarını sağlayarak, yapısal modelin resmi SAP2000 API'si aracılığıyla doğrudan ve eksiksiz bir şekilde aktarımını mümkün kılar. **Güçlü Excel entegrasyonu sayesinde, büyük projelerde veri girişini otomatize ederek zamandan tasarruf etmenizi ve hata riskini minimize etmenizi sağlar.**

---

## Özellikler

Bu uygulama, yapısal modelleme sürecini temelden basitleştiren güçlü ve kapsamlı özellikler sunar:

* **Malzeme Tanımlama:** Beton (örn. C25/30, C30/37) ve donatı çeliği (örn. B420C, B500C) gibi çeşitli malzeme türlerini, karakteristik basınç dayanımı (fck) ve akma (fy) ile çekme (fu) dayanımları gibi özel mühendislik özelliklerini belirterek detaylı bir şekilde tanımlayın ve yönetin.
* **Kesit Tanımlama:** Çeşitli yapısal elemanlar için özelleştirilmiş kesitler oluşturun ve yönetin:
    * **Kolonlar:** Dikdörtgen kolon kesitlerini belirlediğiniz genişlik, yükseklik ve paspayı değerleriyle tanımlayabilir, bu kesitleri önceden tanımlanmış beton ve donatı çeliği malzemeleriyle ilişkilendirebilirsiniz.
    * **Kirişler:** Dikdörtgen kiriş kesitlerini genişlik, yükseklik ve üst/alt paspayı değerleriyle tanımlayabilir, tanımladığınız beton ve donatı çeliği malzemelerini atayabilirsiniz.
    * **Döşemeler:** Kalınlıklarını ve malzemelerini belirleyerek döşeme kesitlerini tanımlayabilirsiniz.
* **Kat Yönetimi:** Toplam kat sayısını, zemin kat (ilk kat) yüksekliğini ve tipik katların yüksekliğini esnek bir şekilde belirleyerek modelinizin düşey yapılandırmasını oluşturun.
* **Eleman Yerleşimi:** Yapısal elemanları sezgisel ve görsel araçlarla kolayca yerleştirin:
    * **Kolon Yerleşimi:** Izgara sistemi üzerinde kolon konumlarını X ve Y koordinatları ile belirleyin ve her bir kolona tanımladığınız kesitleri atayın.
    * **Kiriş Yerleşimi:** Tanımlanmış kolonlar arasında seçim yaparak kiriş elemanları oluşturun ve bu kirişlere uygun kesitleri atayın.
* **Akıllı TBDY-2018 Deprem Tasarımı Entegrasyonu:** Girdiğiniz deprem verilerine (Ss, S1, Yerel Zemin Sınıfı, R, D, I) dayanarak, SAP2000 modelinde TBDY-2018'e göre gerekli tüm yük atamalarını ve analitik ayarları otomatik olarak gerçekleştirir:
    * **Tasarım Spektrumları:** Girilen parametrelere göre hem düşey hem de yatay tasarım spektrum eğrileri otomatik olarak oluşturulur ve modele atanır.
    * **Deprem Yükleri:** Tanımlanan deprem parametreleri doğrultusunda, yönetmeliklere uygun deprem yükleri (eylemsizlik kuvvetleri) yapıya otomatik olarak uygulanır.
    * **Yapı Kütlesi (Mass Source):** Analiz için kritik olan yapı kütlesi tanımlaması (Mass Source), yönetmelik gerekliliklerine birebir uygun şekilde otomatik olarak yapılandırılır.
    * **Yük Kombinasyonları:** Tüm temel yük durumları ve deprem yüklerini içeren, TBDY-2018'e uygun yük kombinasyonları seti otomatik olarak oluşturulur.
* **Excel Entegrasyonu:** Yapısal elemanların konum bilgilerini kesit bilgilerine göre Excel dosyasından hızlıca içeri aktararak manuel veri girişini büyük ölçüde azaltın.
* **SAP2000'e Tek Tıkla Aktarım:** Tanımladığınız tüm yapısal modeli — malzemeler, kesitler, ızgara sistemi, yük desenleri, akıllıca atanmış deprem ve rüzgar yükleri, çerçeve elemanları, mesnet koşulları, kapsamlı yük kombinasyonları ve kütle kaynakları dahil — doğrudan SAP2000'e tek bir tıklamayla aktarır. Bu işlem, SAP2000'in resmi API'si üzerinden yeni bir model örneği oluşturarak gerçekleşir.
* **Modern Kullanıcı Arayüzü:** C# Windows Forms ile geliştirilmiş, sezgisel gezinme ve veri girişi için tasarlanmış, temiz, duyarlı ve kullanıcı dostu bir arayüze sahiptir.

---

## Kullanılan Teknolojiler

* **C#**: Temel programlama dilidir.
* **.NET Framework 4.7.2**: Uygulamayı destekleyen çerçevedir.
* **Windows Forms**: Grafik kullanıcı arayüzünü oluşturmak için kullanılır.
* **SAP2000 API**: SAP2000 ile doğrudan etkileşim için resmi uygulama programlama arayüzü.
* **Newtonsoft.Json**: .NET için yaygın olarak kullanılan bir JSON çerçevesi.

---

## Başlarken

Geliştirme ve test amacıyla projenin yerel makinenizde çalışır duruma gelmesi için aşağıdaki talimatları izleyin.

### Ön Koşullar

Aşağıdaki yazılımların kurulu olduğundan emin olun:

* **Windows İşletim Sistemi**: Uygulama Windows işletim sistemleri için tasarlanmıştır.
* **.NET Framework 4.7.2**: Uygulamayı çalıştırmak için gereklidir.
* **SAP2000 v22**: Model dışa aktarma işlevi için kurulu olması gereken veya API desteği olan uyumlu bir SAP2000 sürümü.
* **Visual Studio 2022**: Projeyi açmak, derlemek ve geliştirmek için önerilir.

### Kurulum

1.  **Depoyu Klonlayın:**
    ```bash
    git clone <depo_url'si>
    ```
    ( `<depo_url'si>` yerine deponuzun gerçek URL'sini yazın)
2.  **Visual Studio'da Açın:**
    Klonladığınız dizine gidin ve Visual Studio 2022'de `.sln` (çözüm) dosyasını açın.
3.  **NuGet Paketlerini Geri Yükleyin:**
    Visual Studio, eksik NuGet paketlerini otomatik olarak geri yüklemenizi istemelidir. İstemezse, Çözüm Gezgini'nde çözüme sağ tıklayın ve "NuGet Paketlerini Geri Yükle" seçeneğini seçin.
4.  **Projeyi Derleyin:**
    Uygulamayı derlemek için çözümü derleyin (Ctrl+Shift+B veya Derle > Çözümü Derle).

---

## Kullanım

Uygulama derlendikten sonra, Visual Studio'dan (F5) çalıştırabilir veya `SapApi/bin/Debug` dizinine giderek `SAP2000.exe` dosyasını çalıştırabilirsiniz.

Yapısal bir model oluşturmak için genel adımları izleyin:

1.  **Malzemeleri Tanımlayın:** "Malzeme Tanımlama" bölümünde, beton ve donatı çeliği malzemelerini ilgili özellikleriyle ekleyin.
2.  **Kesitleri Tanımlayın:** "Kesit Tanımlama" sekmesine gidin. Kolon, kiriş ve döşeme kesitlerini tanımlayın ve yeni oluşturduğunuz malzemeleri bunlara atayın.
3.  **Kat Verilerini Yapılandırın:** "Kat Bilgileri Tanımlama" bölümünde, toplam kat sayısını ve kat yüksekliklerini belirtin.
4.  **Elemanları Yerleştirin:** "Kesit Yerleşimi" sekmesini kullanarak kolonları ve kirişleri manuel olarak yerleştirebilir veya **Excel Entegrasyonunu** kullanabilirsiniz.
5.  **Deprem Bilgilerini Girin:** Deprem verileri formunu açmak için "Deprem Bilgilerini Gir" düğmesine tıklayın. Gerekli TBDY-2018 deprem parametrelerini girin.
6.  **SAP2000'e Aktarın:** Tüm veriler girildikten sonra, "Modeli Sap2000'e Aktar" düğmesine tıklayın. Uygulama SAP2000'e bağlanacak ve modeli oluşturacaktır.

---

## Excel Entegrasyonu

Bu proje, SAP2000 model oluşturma sürecini daha verimli hale getirmek amacıyla güçlü bir Excel entegrasyonu ile geliştirilmiştir. Bu özellik sayesinde, yapısal elemanların (kolonlar ve kirişler) konum ve kesit bilgileri gibi verileri manuel olarak girmek yerine, standart bir Excel dosyasından hızlı ve hatasız bir şekilde içeri aktarabilirsiniz.

### Özelliğin Amacı ve Faydaları

* **Hızlı Veri Girişi:** Özellikle büyük ve karmaşık yapılar için yüzlerce veya binlerce kolon ve kirişin manuel olarak girilmesi zaman alıcı ve hataya açık bir süreçtir. Excel entegrasyonu bu süreci otomatikleştirerek model oluşturma süresini önemli ölçüde kısaltır.
* **Hata Azaltma:** Manuel veri girişindeki insan hatalarını minimize eder. Veriler Excel'de bir kez doğru girildiğinde, uygulama bu verileri tutarlı bir şekilde işler.
* **Veri Tutarlılığı:** Excel'den gelen veriler, projenin mevcut veri modelleriyle uyumlu hale getirilerek SAP2000'e aktarılır, bu da modelin tutarlılığını artırır.
* **Esneklik:** Kullanıcıların kendi tercih ettikleri Excel araçlarını kullanarak verileri hazırlamasına olanak tanır.

### Nasıl Çalışır?

Uygulama, belirli bir formatta hazırlanmış Excel dosyasını okuyarak çalışır. Bu entegrasyonun temel bileşenleri şunlardır:

* **Excel Dosyası Okuma Servisi (`ExcelDataReaderService`):**
    * Bu servis, belirtilen Excel dosyasını açar ve içindeki "Columns" (Kolonlar) ve "Beams" (Kirişler) adlı çalışma sayfalarını arar.
    * Her bir sayfadan, tanımlanmış sütun başlıklarına göre verileri okur.
    * Boş veya geçersiz veri içeren satırları otomatik olarak atlayarak, yalnızca geçerli yapısal bilgileri işler.
    * Sayısal koordinatları (X, Y) okurken, farklı bölgesel ayarlara uyum sağlamak için hem nokta hem de virgül ondalık ayırıcılarını destekler.
* **Veri Modelleri (`ExcelColumnData`, `ExcelBeamData`):**
    * Excel'den okunan veriler, uygulamanın dahili veri yapılarına (`ColumnPlacementInfo`, `BeamPlacementInfo`) dönüştürülmeden önce geçici olarak bu modellere (`ExcelColumnData`, `ExcelBeamData`) eşlenir. Bu, veri işleme sürecini daha düzenli hale getirir.
* **Kullanıcı Arayüzü Entegrasyonu:**
    * Ana formda yer alan "Excel'den İçeri Aktar" düğmesi, kullanıcının bir Excel dosyası seçmesini sağlayan bir `OpenFileDialog` açar.
    * Kullanıcı dosyayı seçtiğinde, `ExcelDataReaderService` devreye girerek verileri okur.
    * Okunan kolon ve kiriş verileri, uygulamanın ana listelerine (`_columnplacements` ve `_beamplacements`) eklenir.
    * Kolon ve kiriş adları Excel'de belirtilmemişse, uygulama otomatik olarak S101, K101 gibi benzersiz adlar atar.
    * Aynı koordinatlarda veya aynı isimde zaten mevcut olan elemanlar için uyarı mesajları gösterilir ve bu elemanlar atlanır.
    * Kirişlerin referans aldığı başlangıç ve bitiş kolonlarının mevcut kolon listesinde olup olmadığı kontrol edilir. Eksik kolonlar varsa, ilgili kirişler atlanır.
    * Veriler başarıyla eklendikten sonra, kullanıcı arayüzündeki ilgili tablolar (kolon ve kiriş yerleşim tabloları) otomatik olarak güncellenir.

### Excel Dosyası Formatı

Excel dosyanızın aşağıdaki formatta olması gerekmektedir:

"**Columns**" (Kolonlar) Sayfası:
Sütun Başlıkları (ilk satırda): `Name` (Kolon Adı), `X` (X Koordinatı - metre), `Y` (Y Koordinatı - metre), `SectionName` (Kolon Kesit Adı)

| Name | X | Y | SectionName |
| :--- | :- | :- | :---------- |
| S101 | 0 | 0 | S30/30 |
| S102 | 5 | 0 | S30/30 |
| ... | ... | ... | ... |

"**Beams**" (Kirişler) Sayfası:
Sütun Başlıkları (ilk satırda): `Name` (Kiriş Adı), `StartColumnName` (Başlangıç Kolonunun Adı), `EndColumnName` (Bitiş Kolonunun Adı), `SectionName` (Kiriş Kesit Adı)

| Name | StartColumnName | EndColumnName | SectionName |
| :--- | :-------------- | :------------ | :---------- |
| K101 | S101 | S102 | K25/50 |
| K102 | S102 | S103 | K25/50 |
| ... | ... | ... | ... |

**Not:** `Name` sütunu boş bırakılabilir; bu durumda uygulama otomatik olarak adlandırma yapacaktır. `X` ve `Y` koordinatları ondalıklı sayıları destekler (virgül veya nokta kullanabilirsiniz). `StartColumnName`, `EndColumnName` ve `SectionName` değerlerinin uygulamada tanımlı olması gerekmektedir.

Bu Excel entegrasyonu, SAP2000 modelleme iş akışınızı önemli ölçüde hızlandıracak ve manuel veri girişinden kaynaklanan hataları azaltacaktır.

---

## Proje Yapısı Genel Bakışı (Geliştiriciler İçin)

Proje, modülerlik, sürdürülebilirlik ve genişletilebilirliği sağlamak için tasarım desenlerinden yararlanan temiz bir mimari tasarıma sahiptir.

* **`SapApi/enums`**: Global numaralandırmaları içerir.
* **`SapApi/factories`**: Malzeme nesnelerini oluşturmak için Fabrika tasarım desenini uygular, gevşek bağlantıyı teşvik eder.
* **`SapApi/models`**: Malzemeler, kesitler, yerleşimler, yükler ve deprem parametreleri için veri yapılarını temsil eden POCO'ları (Plain Old C# Objects) barındırır. Bu, veri bütünlüğünü ve tutarlılığını sağlar.
* **`SapApi/services`**: Temel iş mantığını içerir ve SAP2000 API ile etkileşimleri yönetir.
    * **`ISap2000ApiService` ve `Sap2000ApiService`**: Tüm SAP2000 API işlemleri için arayüzü tanımlar ve uygular.
    * **`builders`**: SAP2000 modelinin belirli kısımlarını (örn. `ColumnBuilder`, `SeismicLoadBuilder`, `GridSystemBuilder`) oluşturma mantığını kapsayan oluşturucu sınıfları koleksiyonudur. Bu, Oluşturucu (Builder) desenine bağlılık, karmaşık nesne oluşturmayı basitleştirir.
    * **`validators`**: İşleme öncesi veri kalitesini sağlayan girdi doğrulama mantığını içerir.
    * **`excel`**: Excel okuma ve veri aktarımı ile ilgili servisleri ve modelleri barındırır (örn. `ExcelDataReaderService`).

---
## Uygulama Tanıtım Videosu

Uygulamanın genel işleyişini ve özelliklerini gösteren tanıtım videosu aşağıdadır.

   https://github.com/user-attachments/assets/fd0c4478-d8dc-4c26-95e8-9c1b2305f028

## Ekran Görüntüleri

<div align="center">







<img src="https://github.com/user-attachments/assets/7c20fea6-9b34-4e15-96cd-88acaaf01ad6" alt="Ana Ekran" width="760" height="480" />
<img src="https://github.com/user-attachments/assets/b56837a8-ac6f-47e5-b997-cf4e6fcdea8d" alt="Kesit Tanımlama/Kesit Yerleşimi" width="760" height="480" />
<img src="https://github.com/user-attachments/assets/9f4cd368-51a7-4d52-b409-24689d50f3f3" alt="Malzeme Tanımlama" width="760" height="480" />

<img src="https://github.com/user-attachments/assets/3be0dbbe-74af-4f28-a5e2-bfa06c244f5f" alt="SAP2000 Açılışı" width="760" height="480" />
<br>
<img src="https://github.com/user-attachments/assets/460d2b0b-e19e-45c1-a68d-95575520ca71" alt="Başarılı Aktarım" width="200" height="120" />
<br>
<img src="https://github.com/user-attachments/assets/ae196838-f2be-4ecb-a631-4687feba73c9" alt="Deprem Verisi Girişi" width="350" height="300" />
<br>
<img src="https://github.com/user-attachments/assets/d2df72e0-5bb8-4aaf-9de8-4d66e1a6000d" alt="SAP2000 Modeli" width="760" height="480" />

</div>

---

## Lisans

Bu proje **MIT Lisansı** altında lisanslanmıştır - ayrıntılar için [LICENSE.md](LICENSE.md) dosyasına bakın.

---

## İletişim

Herhangi bir soru, geri bildirim veya destek talebi için lütfen benimle iletişime geçmekten çekinmeyin:

* **GitHub Sorunları:** Projenin GitHub deposunda [yeni bir sorun](https://github.com/krmsari/sap2000-tbdy2018/issues) oluşturabilirsiniz.
* **E-posta:** `0keremsari@gmail.com`
* **LinkedIn:** [Kerem Sarı](https://www.linkedin.com/in/keremsar/)
