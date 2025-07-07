# SAP2000 TBDY-2018 Model Üretici

Türk Standartı (TS500) ve Türkiye Bina Deprem Yönetmeliğine (TBDY-2018) uygun SAP2000 yapı modellerinin hızlı bir şekilde oluşturulmasını kolaylaştırmak için tasarlanmış modern bir Windows Forms uygulamasıdır. Bu araç, mühendislerin malzemeleri, kesitleri, kat verilerini, eleman yerleşimlerini ve deprem parametrelerini sezgisel olarak tanımlamasına olanak tanır ve böylece tüm yapısal modelin resmi Computers and Structures, Inc. (CSI) şirketinin API aracılığıyla doğrudan SAP2000'e (ETABS ile de kolayca entegre edilebilir) aktarılmasını sağlar.

---

## Özellikler

Bu uygulama, yapısal modelleme sürecini temelden basitleştiren güçlü ve kapsamlı özellikler sunar:

* **Malzeme Tanımlama:** Beton (örn. C25/30, C30/37) ve donatı çeliği (örn. B420C, B500C) gibi çeşitli malzeme türlerini, karakteristik basınç dayanımı (fck) ve akma (fy) ile çekme (fu) dayanımları gibi özel mühendislik özelliklerini belirterek detaylı bir şekilde tanımlayın ve yönetin.
* **Kesit Tanımlama:** Çeşitli yapısal elemanlar için özelleştirilmiş kesitler oluşturun ve yönetin:
    * **Kolonlar:** Dikdörtgen kolon kesitlerini belirlediğiniz genişlik, yükseklik ve paspayı değerleriyle tanımlayabilir, bu kesitleri önceden tanımlanmış beton ve donatı çeliği malzemeleriyle ilişkilendirebilirsiniz.
    * **Kirişler:** Dikdörtgen kiriş kesitlerini genişlik, yükseklik ve üst/alt paspayı değerleriyle tanımlayabilir, tanımladığınız beton ve donatı çeliği malzemelerini atayabilirsiniz.
    * **Döşemeler:** Döşeme kesitlerini kalınlıklarını ve malzemelerini belirterek tanımlayabilirsiniz.
* **Kat Yönetimi:** Toplam kat sayısını, zemin kat (ilk kat) yüksekliğini ve tipik katların yüksekliğini esnek bir şekilde belirleyerek modelinizin düşey yapılandırmasını oluşturun.
* **Eleman Yerleşimi:** Yapısal elemanları sezgisel ve görsel araçlarla kolayca yerleştirin:
    * **Kolon Yerleşimi:** Izgara sistemi üzerinde kolon konumlarını X ve Y koordinatları ile belirleyin ve her bir kolona tanımladığınız kesitleri atayın.
    * **Kiriş Yerleşimi:** Tanımlanmış kolonlar arasında seçim yaparak kiriş elemanları oluşturun ve bu kirişlere uygun kesitleri atayın.
* **Akıllı TBDY-2018 Deprem Tasarımı Entegrasyonu:** Girdiğiniz deprem verilerine (Ss, S1, Yerel Zemin Sınıfı, R, D, I) dayanarak, SAP2000 modelinde TBDY-2018'e göre gerekli tüm yük atamalarını ve analitik ayarları otomatik olarak gerçekleştirir:
    * **Tasarım Spektrumları:** Girilen parametrelere göre hem düşey hem de yatay tasarım spektrum eğrileri otomatik olarak oluşturulur ve modele atanır.
    * **Deprem Yükleri:** Tanımlanan deprem parametreleri doğrultusunda, yönetmeliklere uygun deprem yükleri (eylemsizlik kuvvetleri) yapıya otomatik olarak uygulanır.
    * **Yapı Kütlesi (Mass Source):** Analiz için kritik olan yapı kütlesi tanımlaması (Mass Source), yönetmelik gerekliliklerine birebir uygun şekilde otomatik olarak yapılandırılır.
    * **Yük Kombinasyonları:** Tüm temel yük durumları ve deprem yüklerini içeren, TBDY-2018'e uygun yük kombinasyonları seti otomatik olarak oluşturulur.
* **SAP2000'e Tek Tıkla Aktarım:** Tanımladığınız tüm yapısal modeli — malzemeler, kesitler, ızgara sistemi, yük desenleri, akıllıca atanmış deprem ve rüzgar yükleri, çerçeve elemanları, mesnet koşulları, kapsamlı yük kombinasyonları ve kütle kaynakları dahil — doğrudan SAP2000'e tek bir tıklamayla aktarır. Bu işlem, SAP2000'in resmi API'si üzerinden yeni bir model örneği oluşturarak gerçekleşir.
* **Modern Kullanıcı Arayüzü:** C# Windows Forms ile geliştirilmiş, sezgisel gezinme ve veri girişi için tasarlanmış, temiz, duyarlı ve kullanıcı dostu bir arayüze sahiptir.

---

## Kullanılan Teknolojiler

* **C#**: Temel programlama dilidir.
* **.NET Framework 4.7.2**: Uygulamayı destekleyen çerçevedir.
* **Windows Forms**: Grafik kullanıcı arayüzünü oluşturmak için kullanılır.
* **SAP2000 API**: SAP2000 ile etkileşim için resmi uygulama programlama arayüzü.
* **Newtonsoft.Json**: .NET için popüler bir JSON çerçevesi.

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
4.  **Elemanları Yerleştirin:** "Kesit Yerleşimi" sekmesini kullanarak kolonları ve kirişleri yerleştirin:
    * X ve Y koordinatlarını belirterek ve tanımlanmış bir kolon kesiti atayarak kolonlar ekleyin.
    * Başlangıç ve bitiş kolonlarını seçerek ve tanımlanmış bir kiriş kesiti atayarak kirişler ekleyin.
5.  **Deprem Bilgilerini Girin:** Deprem verileri formunu açmak için "Deprem Bilgilerini Gir" düğmesine tıklayın. Gerekli TBDY-2018 deprem parametrelerini girin.
6.  **SAP2000'e Aktarın:** Tüm veriler girildikten sonra, "Modeli Sap2000'e Aktar" düğmesine tıklayın. Uygulama SAP2000'e bağlanacak ve modeli oluşturacaktır.

---

## Proje Yapısı Genel Bakışı (Geliştiriciler İçin)

Proje, modülerlik, sürdürülebilirlik ve genişletilebilirliği sağlamak için tasarım desenlerinden yararlanan temiz bir mimari tasarıma sahiptir.

* **`SapApi/enums`**: Uygulama genelinde kullanılan genel numaralandırmaları içerir.
* **`SapApi/factories`**: Malzeme nesnelerini oluşturmak için Fabrika tasarım desenini uygular, gevşek bağlantıyı teşvik eder.
* **`SapApi/models`**: Malzemeler, kesitler, yerleşimler, yükler ve deprem parametreleri için veri yapılarını temsil eden Düz Eski C# Nesnelerini (POCO'lar) barındırır. Bu, veri bütünlüğünü ve tutarlılığını sağlar.
* **`SapApi/services`**: Temel iş mantığını içerir ve SAP2000 API ile etkileşimleri yönetir.
    * **`ISap2000ApiService` ve `Sap2000ApiService`**: Tüm SAP2000 API işlemleri için arayüzü tanımlar ve uygular.
    * **`builders`**: SAP2000 modelinin belirli kısımlarını (örn. `ColumnBuilder`, `SeismicLoadBuilder`, `GridSystemBuilder`) oluşturma mantığını kapsayan oluşturucu sınıfları koleksiyonudur. Bu, Oluşturucu (Builder) desenine bağlılık, karmaşık nesne oluşturmayı basitleştirir.
    * **`validators`**: İşleme öncesi veri kalitesini sağlayan girdi doğrulama mantığını içerir.

---

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

Herhangi bir soru veya destek için lütfen GitHub deposunda bir sorun açın veya [Kerem Sarı/0keremsari@gmail.com/Profil Bağlantınız] ile iletişime geçin.
