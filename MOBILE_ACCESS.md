# Mobil Səhifəyə Giriş Təlimatı

## Mobil Səhifəyə Necə Girmək Olar?

### 1. Avtomatik Yönləndirmə
- Login olduqdan sonra avtomatik olaraq `/mobile` səhifəsinə yönləndiriləcəksiniz
- Əgər zaten login olmusunuzsa, `/` URL-inə getdikdə avtomatik `/mobile`-ə yönləndiriləcəksiniz

### 2. Birbaşa URL
- Browser-də birbaşa `/mobile` yazın
- Məsələn: `http://localhost:5173/mobile`

### 3. Mobile Navbar
- Aşağıdakı bottom navigation bar-da "Ana səhifə" ikonuna basın
- Bu sizi `/mobile` səhifəsinə aparacaq

## Mobil Səhifənin Xüsusiyyətləri

### GPS İcazəsi
- İlk dəfə səhifəyə daxil olduqda GPS icazəsi soruşulacaq
- İcazə verildikdə, ətrafınızdakı restoranlar və təkliflər göstəriləcək
- Radius: 5km (avtomatik)

### Əsas Funksiyalar
1. **Salam Banner** - Profil məlumatları ilə
2. **Axtarış Bar** - Təklifləri axtarmaq üçün
3. **Təklif Carousel** - Aktiv təkliflər
4. **Kateqoriyalar** - Horizontal scroll
5. **Populyar Təkliflər** - Grid görünüşü
6. **Ətrafımdakı Restoranlar** - GPS əsaslı

### Navigation
- **Ana səhifə** (🏠) - `/mobile`
- **Sevimlilər** (❤️) - `/favorites`
- **Sifarişlər** (🛍️) - `/orders`
- **Profil** (👤) - `/profile`

## Test Etmək Üçün

1. Frontend-i işə salın: `npm run dev`
2. Login olun
3. `/mobile` səhifəsinə gedin
4. GPS icazəsi verin
5. Təklifləri və restoranları görün

## Qeyd
- Mobil səhifə iPhone 13 ölçülərinə (390x844px) uyğunlaşdırılıb
- Desktop-da da işləyir, amma mobil üçün optimizasiya edilib




