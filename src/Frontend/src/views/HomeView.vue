<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import CyberParticles from '../components/CyberParticles.vue';
import apiClient from '../utils/api';
import { ArrowRight, ShoppingBag, Zap, ShieldCheck } from 'lucide-vue-next';
import anime from 'animejs';

// State
const products = ref<any[]>([]);
const loading = ref(true);
const error = ref('');
const timeLeft = ref({ hours: '02', minutes: '00', seconds: '00' });
let intervalId: any = null;
let pollInterval: any = null;

const startCountdown = () => {
    let duration = 2 * 60 * 60; // 2 hours in seconds
    intervalId = setInterval(() => {
        const hours = Math.floor(duration / 3600);
        const minutes = Math.floor((duration % 3600) / 60);
        const seconds = duration % 60;

        timeLeft.value = {
            hours: String(hours).padStart(2, '0'),
            minutes: String(minutes).padStart(2, '0'),
            seconds: String(seconds).padStart(2, '0')
        };

        if (--duration < 0) {
            duration = 2 * 60 * 60; // Reset loop
        }
    }, 1000);
};

// Updated Categories for 2025 Look
const categories = [
    { id: 1, name: 'Streetwear', image: 'https://images.unsplash.com/photo-1523398002811-999ca8dec234?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '120+', span: 'md:col-span-2 md:row-span-2' },
    { id: 2, name: 'Accessories', image: 'https://images.unsplash.com/photo-1576053139778-7e32f2ae3cfd?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '80+', span: 'md:col-span-1 md:row-span-1' },
    { id: 3, name: 'Sneakers', image: 'https://images.unsplash.com/photo-1552346154-21d32810aba3?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '45+', span: 'md:col-span-1 md:row-span-1' },
    { id: 4, name: 'Collections', image: 'https://images.unsplash.com/photo-1483985988355-763728e1935b?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '240+', span: 'md:col-span-2 md:row-span-1' },
];

// Fetch Data
const fetchProducts = async (isBackground = false) => {
    if (!isBackground) loading.value = true;
    try {
        const response: any = await apiClient.get('/products');
        let allItems = response as any[];

        // Config: Keywords to identify underwear/accessories (to deprioritize)
        const secondaryKeywords = ['lót', 'sịp', 'boxer', 'underwear', 'brief', 'tất', 'sock'];
        
        // Sort: Main clothing first, Secondary items last
        allItems.sort((a, b) => {
            const nameA = (a.name || '').toLowerCase();
            const nameB = (b.name || '').toLowerCase();
            // const catA = (a.categoryName || '').toLowerCase(); // If categoryName exists

            const isSecondaryA = secondaryKeywords.some(k => nameA.includes(k));
            const isSecondaryB = secondaryKeywords.some(k => nameB.includes(k));

            if (isSecondaryA && !isSecondaryB) return 1; // A goes after B
            if (!isSecondaryA && isSecondaryB) return -1; // A goes before B
            return 0; // Keep original order otherwise
        });

        // Get only first 8 products for Home (now showing main clothes first)
        products.value = allItems.slice(0, 8); 
    } catch (err) {
        if (!isBackground) error.value = 'Không thể tải sản phẩm. Vui lòng kiểm tra kết nối.';
        console.error(err);
    } finally {
        if (!isBackground) loading.value = false;
    }
};

const animateText = () => {
    // Wrap every letter in a span
    const textWrapper = document.querySelector('.ml11 .letters');
    if (textWrapper && textWrapper.textContent) {
        textWrapper.innerHTML = textWrapper.textContent.replace(/([^\x00-\x80]|\w)/g, "<span class='letter'>$&</span>");

        anime.timeline({loop: false})
        .add({
            targets: '.ml11 .letter',
            translateY: ["2.1em", 0],
            translateZ: 0,
            duration: 950,
            delay: (_el: any, i: number) => 60 * i,
            easing: "easeOutExpo"
        });
    }
}

const setTheme = (theme: string) => {
    document.documentElement.setAttribute('data-theme', theme);
    // Optional: Save to localStorage for persistence
    localStorage.setItem('coolstyle-theme', theme);
};

onMounted(() => {
    fetchProducts();
    startCountdown();
    animateText();
    
    // Load saved theme
    const savedTheme = localStorage.getItem('coolstyle-theme') || 'default';
    setTheme(savedTheme);

    // Auto-refresh data every 5 seconds (to catch new images)
    pollInterval = setInterval(() => {
        fetchProducts(true);
    }, 5000);
});

onUnmounted(() => {
    if (intervalId) clearInterval(intervalId);
    if (pollInterval) clearInterval(pollInterval);
});
</script>

<template>
  <MainLayout>
    <!-- 1. Hero Section (Modern Fashion Design + Three.js + Anime.js) -->
    <section class="relative min-h-[90vh] flex items-center bg-transparent overflow-hidden pt-16">
        
        <!-- Three.js Background -->
        <CyberParticles />

        <!-- Background Elements (Fallback/Enhancement) -->
        <div class="absolute top-0 right-0 w-[600px] h-[600px] bg-gradient-to-b from-cyan-500/10 to-transparent rounded-full blur-[120px] -translate-y-1/2 translate-x-1/2 pointer-events-none"></div>
        <div class="absolute bottom-0 left-0 w-[500px] h-[500px] bg-purple-600/10 rounded-full blur-[100px] translate-y-1/2 -translate-x-1/4 pointer-events-none"></div>

        <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-2 gap-12 items-center relative z-10 pointer-events-none">
            
            <!-- Left: Content (Pointer Events Auto to interact with buttons) -->
            <div class="pointer-events-auto" data-aos="fade-right" data-aos-duration="1000">
                <div class="flex items-center gap-3 mb-6">
                    <span class="w-12 h-[2px] bg-theme-primary"></span>
                    <span class="text-theme-primary font-bold uppercase tracking-[0.3em] text-sm">Trend 2025</span>
                </div>

                <h1 class="ml11 font-display text-6xl md:text-8xl font-black text-white leading-[0.9] mb-8 overflow-hidden">
                    <span class="letters block">STREET WEAR.</span>
                    <span class="block text-4xl md:text-5xl font-light text-gray-400 mt-2 italic opacity-0 animate-fade-in" style="animation-delay: 1s; animation-fill-mode: forwards;">Collection</span>
                </h1>

                <p class="text-gray-400 text-lg mb-10 max-w-lg leading-relaxed border-l-2 border-white/10 pl-6 animate-fade-in-up" style="animation-delay: 0.5s">
                    Định hình phong cách cá nhân với những thiết kế độc bản. Sự kết hợp hoàn hảo giữa công nghệ và thời trang đường phố.
                </p>

                <div class="flex items-center gap-6 animate-fade-in-up" style="animation-delay: 0.7s">
                    <button class="btn-primary bg-white text-black hover:bg-theme-primary hover:text-black border-none px-10 py-4 text-lg shadow-[0_0_20px_rgba(255,255,255,0.2)]">
                        Shop Now
                    </button>
                    <button class="flex items-center gap-3 text-white font-bold hover:text-theme-primary transition group">
                        <span class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center group-hover:bg-theme-primary group-hover:text-black group-hover:border-theme-primary transition-all">
                             <ArrowRight :size="18" />
                        </span>
                        <span>View Lookbook</span>
                    </button>
                </div>
                
                <!-- Theme Switcher (Demo Only) -->
                <div class="mt-8 flex gap-4 animate-fade-in-up" style="animation-delay: 0.9s">
                    <button @click="setTheme('default')" class="w-8 h-8 rounded-full bg-[#00f2ea] border-2 border-white/50 hover:scale-110 transition shadow-[0_0_10px_#00f2ea]" title="Cyberpunk"></button>
                    <button @click="setTheme('gold')" class="w-8 h-8 rounded-full bg-[#FFD700] border-2 border-white/50 hover:scale-110 transition shadow-[0_0_10px_#FFD700]" title="Luxury Gold"></button>
                    <button @click="setTheme('acid')" class="w-8 h-8 rounded-full bg-[#CCFF00] border-2 border-white/50 hover:scale-110 transition shadow-[0_0_10px_#CCFF00]" title="Acid Green"></button>
                    <button @click="setTheme('sunset')" class="w-8 h-8 rounded-full bg-[#FF5F1F] border-2 border-white/50 hover:scale-110 transition shadow-[0_0_10px_#FF5F1F]" title="Sunset Vapor"></button>
                    <button @click="setTheme('future')" class="w-8 h-8 rounded-full bg-[#FFFFFF] border-2 border-white/50 hover:scale-110 transition shadow-[0_0_10px_#FFFFFF]" title="Future Mono"></button>
                </div>

                <!-- Stats / Trust -->
                <div class="mt-16 grid grid-cols-3 gap-8 animate-fade-in" style="animation-delay: 1s">
                    <div>
                        <h4 class="text-3xl font-bold text-white font-display">2k+</h4>
                        <p class="text-gray-500 text-xs uppercase tracking-wider mt-1">New Drops</p>
                    </div>
                    <div>
                        <h4 class="text-3xl font-bold text-white font-display">50k+</h4>
                        <p class="text-gray-500 text-xs uppercase tracking-wider mt-1">Happy Users</p>
                    </div>
                    <div>
                        <h4 class="text-3xl font-bold text-white font-display">100%</h4>
                        <p class="text-gray-500 text-xs uppercase tracking-wider mt-1">Authentic</p>
                    </div>
                </div>
            </div>

            <!-- Right: Visual -->
            <div class="relative h-full flex justify-center lg:justify-end pointer-events-auto" data-aos="fade-left" data-aos-duration="1200">
                <!-- Decorative Circle -->
                <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[400px] h-[400px] md:w-[600px] md:h-[600px] border border-white/10 rounded-full animate-[spin_30s_linear_infinite]"></div>
                
                <!-- Main Image Masked -->
                <div class="relative z-10 w-[350px] md:w-[450px] h-[500px] md:h-[650px] rounded-t-[200px] rounded-b-[40px] overflow-hidden border-4 border-white/5 bg-[#111]">
                    <img src="https://images.unsplash.com/photo-1552346154-21d32810aba3?q=80&w=1500&auto=format&fit=crop" 
                         class="w-full h-full object-cover hover:scale-105 transition duration-700" alt="Hoodie Streetwear Model">
                    
                    <!-- Floating Card -->
                    <div class="absolute bottom-8 left-8 right-8 glass p-4 rounded-xl flex items-center gap-4 animate-float">
                        <div class="w-12 h-12 rounded-full bg-white flex items-center justify-center text-black font-bold">
                            <ShoppingBag :size="20"/>
                        </div>
                        <div>
                            <p class="text-white font-bold text-sm">Air Jordan 1 Retro High OG ‘Chicago’ 2015</p>
                            <p class="text-theme-primary font-bold text-xs">$89.00</p>
                        </div>
                        <button class="ml-auto w-8 h-8 rounded-full bg-white/10 flex items-center justify-center hover:bg-theme-primary hover:text-black transition">
                            <ArrowRight :size="14"/>
                        </button>
                    </div>
                </div>

                <!-- Floating Tags -->
                <div class="absolute top-20 -left-10 glass px-4 py-2 rounded-full text-xs font-bold text-white animate-float" style="animation-delay: 1s">
                    ✨ New Season
                </div>
                <div class="absolute bottom-40 -right-4 glass px-4 py-2 rounded-full text-xs font-bold text-white animate-float" style="animation-delay: 2s">
                    🔥 50% OFF
                </div>
            </div>
        </div>
    </section>

    <!-- 2. Trending Categories (Bento Grid) -->
    <section class="py-32 bg-transparent relative z-10">
        <div class="container mx-auto px-6">
             <div class="flex justify-between items-end mb-16" data-aos="fade-up">
                 <div>
                     <h2 class="font-display text-4xl md:text-5xl font-bold mb-4">Danh Mục <span class="text-neon">Trending</span></h2>
                     <p class="text-gray-400 text-lg">Những items được săn đón nhiều nhất</p>
                 </div>
                 <button class="text-white hover:text-theme-primary transition font-medium flex items-center gap-2">
                    Khám phá tất cả <ArrowRight :size="18"/>
                 </button>
             </div>

             <div class="grid grid-cols-1 md:grid-cols-4 grid-rows-2 gap-6 h-auto md:h-[600px]">
                 <div v-for="(cat, index) in categories" :key="cat.id" 
                      :class="['group relative rounded-3xl overflow-hidden cursor-pointer border border-white/5', cat.span]"
                      :data-aos="'fade-up'" :data-aos-delay="index * 100">
                      
                      <img :src="cat.image" class="w-full h-full object-cover transition duration-700 group-hover:scale-110 opacity-70 group-hover:opacity-100" :alt="cat.name">
                      
                      <!-- Overlay Gradient -->
                      <div class="absolute inset-0 bg-gradient-to-t from-black via-black/50 to-transparent"></div>
                      
                      <!-- Content -->
                      <div class="absolute bottom-6 left-6 z-10">
                          <span class="px-2 py-1 bg-white/10 backdrop-blur text-xs rounded text-white mb-2 inline-block border border-white/10">{{ cat.count }} Items</span>
                          <h3 class="text-3xl font-display font-bold text-white group-hover:text-theme-primary transition-colors">{{ cat.name }}</h3>
                      </div>
                  </div>
             </div>
        </div>
    </section>

    <!-- 2.5 Flash Sale / Best Sellers -->
    <section class="py-24 bg-theme-gradient relative overflow-hidden border-y border-white/5">
        <!-- Background Effects -->
        <div class="absolute -left-20 top-0 w-96 h-96 bg-purple-600/20 rounded-full blur-[100px]"></div>
        <div class="absolute right-0 bottom-0 w-80 h-80 bg-cyan-500/10 rounded-full blur-[80px]"></div>

        <div class="container mx-auto px-6 relative z-10">
            <div class="flex flex-col md:flex-row items-end justify-between mb-12 gap-8">
                <!-- Header & Timer -->
                <div>
                    <div class="flex items-center gap-3 mb-2">
                        <Zap class="text-yellow-400 fill-yellow-400 animate-pulse" :size="24" />
                        <span class="text-yellow-400 font-bold tracking-[0.2em] uppercase text-sm">Flash Sale Ends In</span>
                    </div>
                    <div class="flex flex-wrap items-center gap-6">
                        <h2 class="font-display text-5xl md:text-7xl font-black text-white italic tracking-tighter drop-shadow-[0_0_15px_rgba(255,255,255,0.3)]">BEST SELLERS</h2>
                        <!-- Countdown -->
                        <div class="flex gap-1 text-3xl md:text-5xl font-mono font-bold text-white bg-white/5 px-6 py-2 rounded-xl backdrop-blur border border-white/10">
                            <span>{{ timeLeft.hours }}</span><span class="animate-pulse text-purple-400">:</span>
                            <span>{{ timeLeft.minutes }}</span><span class="animate-pulse text-purple-400">:</span>
                            <span>{{ timeLeft.seconds }}</span>
                        </div>
                    </div>
                </div>
                
                <button class="flex items-center gap-2 text-theme-secondary font-bold hover:text-white transition group text-lg">
                    View All Deals <ArrowRight :size="20" class="group-hover:translate-x-1 transition-transform"/>
                </button>
            </div>

            <!-- Products Slider (Grid for now) -->
             <div v-if="loading" class="grid grid-cols-2 md:grid-cols-4 gap-6">
                <div v-for="i in 4" :key="i" class="aspect-[3/4] bg-white/5 rounded-2xl animate-pulse"></div>
             </div>
             
             <div v-else class="grid grid-cols-2 md:grid-cols-4 gap-6">
                 <!-- Showing reversed slice for variety -->
                 <ProductCard v-for="(product, index) in [...products].reverse().slice(0, 4)" :key="product.id" :product="product" 
                              :data-aos="'fade-up'" :data-aos-delay="index * 100" />
             </div>
        </div>
    </section>

    <!-- 3. New Arrivals -->
    <section class="py-32 bg-black/20 backdrop-blur-sm border-y border-white/5 relative z-10">
        <div class="container mx-auto px-6 relative z-10">
             <div class="text-center mb-20" data-aos="fade-up">
                 <span class="text-theme-primary font-bold uppercase tracking-[0.2em] text-sm mb-4 block">New Drops</span>
                 <h2 class="font-display text-5xl font-black text-white mb-6">Mới Lên Kệ</h2>
                 <p class="text-gray-400 text-lg max-w-2xl mx-auto">Cập nhật xu hướng thời trang mới nhất. Đừng bỏ lỡ những items giới hạn.</p>
             </div>

             <!-- Loading -->
             <div v-if="loading" class="grid grid-cols-1 md:grid-cols-4 gap-8">
                <div v-for="i in 4" :key="i" class="aspect-[3/4] bg-white/5 rounded-2xl animate-pulse"></div>
             </div>
             
             <!-- Products Grid -->
             <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
                 <ProductCard v-for="(product, index) in products" :key="product.id" :product="product" 
                              :data-aos="'fade-up'" :data-aos-delay="index * 100" />
             </div>
             
             <div class="text-center mt-16">
                 <button class="btn-ghost">Xem Thêm Sản Phẩm</button>
             </div>
        </div>
    </section>

    <!-- 4. Features (Quality Promise) -->
    <section class="py-32 bg-black/40 backdrop-blur-md relative overflow-hidden border-t border-white/5">
        <!-- Glows -->
        <div class="absolute top-0 left-0 w-[500px] h-[500px] bg-blue-600/10 rounded-full blur-[120px]"></div>
        <div class="absolute bottom-0 right-0 w-[500px] h-[500px] bg-purple-600/10 rounded-full blur-[120px]"></div>
        
        <div class="container mx-auto px-6 relative z-10">
             <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
                 <!-- Feature 1 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-theme-primary/20 group-hover:border-theme-primary/50 transition-colors">
                         <Zap :size="32" class="text-theme-primary" />
                     </div>
                     <h3 class="text-xl font-bold text-white mb-4 font-display">Fast Delivery</h3>
                     <p class="text-gray-400 leading-relaxed">Giao hàng hỏa tốc nội thành trong 2h. Đóng gói cao cấp bảo vệ sản phẩm.</p>
                 </div>

                 <!-- Feature 2 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up" data-aos-delay="100">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-theme-secondary/20 group-hover:border-theme-secondary/50 transition-colors">
                         <ShieldCheck :size="32" class="text-theme-secondary" />
                     </div>
                     <h3 class="text-xl font-bold text-white mb-4 font-display">Premium Quality</h3>
                     <p class="text-gray-400 leading-relaxed">Cam kết chính hãng 100%. Bảo hành đường may trọn đời.</p>
                 </div>

                 <!-- Feature 3 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up" data-aos-delay="200">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-theme-accent/20 group-hover:border-theme-accent/50 transition-colors">
                         <ShoppingBag :size="32" class="text-theme-accent" />
                     </div>
                     <h3 class="text-xl font-bold text-white mb-4 font-display">Easy Returns</h3>
                     <p class="text-gray-400 leading-relaxed">Đổi trả không cần lý do trong 30 ngày. Hoàn tiền ngay lập tức.</p>
                 </div>
             </div>
        </div>
    </section>
  </MainLayout>
</template>

<style scoped>
.ml11 .letter {
  display: inline-block;
  line-height: 1em;
}
</style>
