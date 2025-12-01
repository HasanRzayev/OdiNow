# 📱 Mobil Səhifəyə Giriş Təlimatı

## 🚀 Mobil Səhifəyə Necə Girmək Olar?

### 1️⃣ Avtomatik Yönləndirmə (Ən Asan)
- **Login olduqdan sonra** avtomatik olaraq `/mobile` səhifəsinə yönləndiriləcəksiniz
- Əgər zaten login olmusunuzsa, `/` URL-inə getdikdə avtomatik `/mobile`-ə yönləndiriləcəksiniz

### 2️⃣ Birbaşa URL
Browser-də birbaşa yazın:
```
http://localhost:5173/mobile
```
və ya production-da:
```
https://yourdomain.com/mobile
```

### 3️⃣ Mobile Navbar
Aşağıdakı bottom navigation bar-da **"Ana səhifə" (🏠)** ikonuna basın

### 4️⃣ Link vasitəsilə
Hər hansı bir səhifədən `/mobile` linkinə basın

---

## 📋 Mobil Səhifənin Xüsusiyyətləri

### 📍 GPS İcazəsi
- ✅ İlk dəfə səhifəyə daxil olduqda **GPS icazəsi avtomatik soruşulacaq**
- ✅ İcazə verildikdə, **ətrafınızdakı restoranlar və təkliflər** göstəriləcək
- ✅ **Radius: 5km** (avtomatik təyin olunur)
- ✅ GPS icazəsi verilməsə, bütün təkliflər göstəriləcək

### 🎯 Əsas Funksiyalar

1. **Salam Banner** 🎉
   - Profil məlumatları ilə
   - Bildiriş ikonu

2. **Axtarış Bar** 🔍
   - Təklifləri axtarmaq üçün
   - Filter ikonu

3. **Təklif Carousel** 🎠
   - Aktiv təkliflər
   - Hər 30 saniyədə bir dəyişir

4. **Kateqoriyalar** 📂
   - Horizontal scroll
   - Kateqoriyaya basanda detallı səhifə açılır

5. **Populyar Təkliflər** ⭐
   - Grid görünüşü (2 sütun)
   - Şəkillərlə

6. **Ətrafımdakı Restoranlar** 🏪
   - GPS əsaslı
   - Məsafə ilə
   - Şəkillərlə

### 🧭 Navigation (Bottom Bar)

- **Ana səhifə** (🏠) → `/mobile`
- **Sevimlilər** (❤️) → `/favorites`
- **Sifarişlər** (🛍️) → `/orders`
- **Profil** (👤) → `/profile`

---

## 🧪 Test Etmək Üçün

### Adım 1: Frontend-i işə salın
```bash
cd OdiNow-Frontend
npm run dev
```

### Adım 2: Backend-i işə salın
```bash
cd OdiNow
dotnet run
```

### Adım 3: Login olun
- Browser-də `http://localhost:5173/login` açın
- Login olun

### Adım 4: Mobil səhifəyə gedin
- Avtomatik `/mobile`-ə yönləndiriləcəksiniz
- Və ya birbaşa `/mobile` yazın

### Adım 5: GPS icazəsi verin
- Browser GPS icazəsi soruşacaq
- "Allow" basın

### Adım 6: Təklifləri görün
- Ətrafınızdakı təkliflər və restoranlar görünəcək

---

## 📱 Responsive Dizayn

- **iPhone 13 ölçüləri**: 390x844px
- **Max width**: 390px
- **Mobile-first** dizayn
- Desktop-da da işləyir, amma mobil üçün optimizasiya edilib

---

## ⚙️ Konfiqurasiya

### GPS Radius
Default radius: **5000 metr (5km)**

Dəyişdirmək üçün `MobileHome.jsx`-də:
```javascript
const radius = 5000; // metrlə
```

### Ticket Sistemi
- Hər 30 dəqiqədə bir yeni ticket
- Maksimum 5 aktiv ticket
- Eyni təklifə ikinci dəfə baxanda ticket istifadə olunmur

---

## 🐛 Problem Həlləri

### GPS işləmir?
- Browser settings-də location permission yoxlayın
- HTTPS istifadə edin (localhost-da işləyir)
- Browser console-da error yoxlayın

### Təkliflər görünmür?
- GPS icazəsi verildiyinə əmin olun
- Backend-in işlədiyinə əmin olun
- Browser console-da error yoxlayın

### Səhifə boşdur?
- Network tab-da API request-ləri yoxlayın
- Backend log-larına baxın
- Token-in valid olduğuna əmin olun

---

## 📞 Əlavə Məlumat

Ətraflı məlumat üçün `MOBILE_ACCESS.md` faylına baxın.




