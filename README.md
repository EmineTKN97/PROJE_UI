# KÜLTÜRİX (BLOG-HABER-BİLET) SİTESİ FRONTEND KISIMI

Yaptığım Kültürix projesinin frontend projesidir.Asp.Net MVC teknolojisi ile oluşturdu.Model kısımında backend de oluşturduğum DTO'ları mapledim.
## Kurulum
Projenin yerel olarak nasıl kurulacağına dair adımlar:
1. Proje kurulumu yaptıktan sonra Müzeleri çekebilmek için
[https://www.nosyapi.com](https://www.nosyapi.com)sitesinden ücretsiz müze api kısımını kullandım.Sizde kayıt olup buradan bir apikey almalısınız.Aldığınız apikeyi Views/Bilet/Bilet.cshtml.dosyasına yapıştırmalısınız.
![klasör yolu](https://github.com/EmineTKN97/ProjeOdev_KULTURIX/assets/156480828/bf4fadea-a656-4c86-90d7-a0a91ccdd71d)
![apikey](https://github.com/EmineTKN97/ProjeOdev_KULTURIX/assets/156480828/eb99f90f-d890-4514-bc8e-5b9b56b5d2a3)Buradaki yıldızlı yere keyi yapıştırmalısınız.
3.Ödeme kısımı için Stripe Payment kütüphanesini kullandım.Bunun içinde [https://stripe.com/](https://stripe.com/) sayfasına developers olarak kayıt olarak secretkey ve publickey almanız gerekir.Bu keyleri projenin appsetting.json
dosyasına eklemeniz gerekir.
![key](https://github.com/EmineTKN97/ProjeOdev_KULTURIX/assets/156480828/463559bf-55de-44af-9c3b-2f0c6a77eb85)Buradaki yıldızlı kısımlara keyi eklmeniz gerekir.
4.Program.cs dosyasındaki baseurl kısımını kend url'nizi eklemelisiniz.Views dosyasındaki bazı url kısımlarını buna göre düzenlemeniz gerekir.

### Kullanılan Teknolojiler

- **MVC (Model-View-Controller):** Projenin frontend yapısı, MVC mimarisine dayanmaktadır. Bu sayede kodun daha modüler ve yönetilebilir olması sağlanmıştır.
- **Stripe Payment:** Ödeme işlemleri için Stripe Payment API'si entegre edilmiştir. Kullanıcılar etkinliklere katılım için ödeme yapabilir ve güvenli bir şekilde işlem gerçekleştirebilirler.

### Nasıl Çalışır?

1. Kullanıcı etkinliği satın almak istediğinde, ödeme ekranına yönlendirilir.
2. Stripe Payment API'si kullanılarak güvenli bir şekilde ödeme işlemi gerçekleştirilir.
3. Ödeme başarılı olduğunda kullanıcı etkinliğe katılabilir ve biletini alabilir.

Frontend kısmında kullanılan teknolojilerin ve ödeme işlemlerinin detaylı incelemesi için lütfen projenin ilgili kod bölümlerini ve belgelerini inceleyebilirsiniz.

  
## Katkıda Bulunma

- Eğer projeye katkıda bulunmak istiyorsanız, öncelikle bir issue açarak konuyu belirtin.
- Fork ederek kendi çalışma alanınıza kopyalayın.
- Yaptığınız değişiklikleri yeni bir branch oluşturarak commit edin.
- Pull request (çekme isteği) gönderin ve değişikliklerinizi tartışın.

Bu teknolojilerin bir araya gelmesiyle proje geliştirme süreci daha verimli ve etkili hale getirilmiştir. Detaylı bilgi için lütfen proje klasöründe yer alan belgelere ve kodlara göz atabilirsiniz.
Proje hakkında herhangi bir sorunuz, geri bildiriminiz veya işbirliği teklifiniz varsa benimle iletişime geçebilirsiniz.

- **E-posta:** [eminetkn.97@gmail.com](eminetkn.97@gmail.com)
- Her türlü geri bildiriminiz benim için değerlidir. Teşekkür ederim!
