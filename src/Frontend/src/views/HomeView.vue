<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { ArrowRight, ShoppingBag, Zap, ShieldCheck } from 'lucide-vue-next';

// State
const products = ref<any[]>([]);
const loading = ref(true);
const error = ref('');

// Updated Categories for 2025 Look
const categories = [
    { id: 1, name: 'Streetwear', image: 'https://images.unsplash.com/photo-1523398002811-999ca8dec234?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '120+', span: 'md:col-span-2 md:row-span-2' },
    { id: 2, name: 'Accessories', image: 'https://images.unsplash.com/photo-1576053139778-7e32f2ae3cfd?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '80+', span: 'md:col-span-1 md:row-span-1' },
    { id: 3, name: 'Sneakers', image: 'https://images.unsplash.com/photo-1552346154-21d32810aba3?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '45+', span: 'md:col-span-1 md:row-span-1' },
    { id: 4, name: 'Collections', image: 'https://images.unsplash.com/photo-1483985988355-763728e1935b?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '240+', span: 'md:col-span-2 md:row-span-1' },
];

// Fetch Data
const fetchProducts = async () => {
    try {
        const response: any = await apiClient.get('/products');
        // Get only first 8 products for Home
        products.value = (response as any[]).slice(0, 8); 
    } catch (err) {
        error.value = 'Không thể tải sản phẩm. Vui lòng kiểm tra kết nối.';
        console.error(err);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    fetchProducts();
});
</script>

<template>
  <MainLayout>
    <!-- 1. Hero Section (Neo-Glass Dark Mode) -->
    <div class="relative bg-black min-h-screen flex items-center overflow-hidden">
        
        <!-- Animated Background Blobs -->
        <div class="absolute top-0 left-1/2 -translate-x-1/2 w-[800px] h-[800px] bg-purple-600/20 rounded-full blur-[120px] animate-pulse"></div>
        <div class="absolute bottom-0 right-0 w-[600px] h-[600px] bg-cyan-500/10 rounded-full blur-[100px]"></div>

        <div class="container mx-auto px-6 z-10 grid grid-cols-1 lg:grid-cols-2 gap-16 items-center pt-20">
            <!-- Text Content -->
            <div data-aos="fade-right" data-aos-duration="1000">
                <div class="inline-flex items-center gap-2 px-4 py-2 rounded-full border border-cyan-500/30 bg-cyan-900/10 backdrop-blur-md mb-8">
                    <span class="relative flex h-2 w-2">
                      <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
                      <span class="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
                    </span>
                    <span class="text-cyan-300 text-xs font-bold uppercase tracking-widest">New Collection 2025</span>
                </div>

                <h1 class="font-display text-7xl md:text-9xl font-black leading-[0.9] tracking-tighter mb-8 text-white">
                    FUTURE <br> 
                    <span class="text-transparent bg-clip-text bg-gradient-to-r from-[#00f2ea] to-[#4361ee]">FASHION.</span>
                </h1>
                
                <p class="text-xl text-gray-400 mb-10 max-w-lg font-light leading-relaxed">
                    Khám phá phong cách Neo-Streetwear đậm chất tương lai. Thiết kế tối giản, công nghệ vải tiên tiến và trải nghiệm mua sắm đẳng cấp.
                </p>
                
                <div class="flex flex-wrap gap-6">
                    <button class="btn-primary flex items-center gap-3 group">
                        Mua ngay 
                        <ArrowRight :size="20" class="group-hover:translate-x-1 transition-transform"/>
                    </button>
                    <button class="btn-ghost backdrop-blur-md flex items-center gap-2">
                        Xem Bộ Sưu Tập
                    </button>
                </div>

                <!-- Social Proof -->
                <div class="mt-12 flex items-center gap-4 text-gray-500 text-sm font-medium">
                    <div class="flex -space-x-4">
                        <img class="w-10 h-10 rounded-full border-2 border-black" src="https://i.pravatar.cc/100?img=1" alt="User">
                        <img class="w-10 h-10 rounded-full border-2 border-black" src="https://i.pravatar.cc/100?img=2" alt="User">
                        <img class="w-10 h-10 rounded-full border-2 border-black" src="https://i.pravatar.cc/100?img=3" alt="User">
                    </div>
                    <span>Được yêu thích bởi 10,000+ Fashionistas</span>
                </div>
            </div>

            <!-- 3D Card / Visual -->
            <div class="hidden lg:block relative" data-aos="fade-left" data-aos-duration="1200" data-aos-delay="200">
                 <!-- Main Floating Card -->
                 <div class="relative z-10 glass-card p-2 transform rotate-6 hover:rotate-2 transition-transform duration-700 w-[450px] mx-auto animate-float">
                      <div class="relative overflow-hidden rounded-xl">
                          <img src="https://images.unsplash.com/photo-1617137968427-85924c800a22?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80" 
                               class="w-full h-[600px] object-cover" alt="Featured Look">
                          
                          <!-- Overlay Info -->
                          <div class="absolute bottom-0 inset-x-0 p-6 bg-gradient-to-t from-black/90 to-transparent pt-20">
                              <h3 class="text-3xl font-bold text-white font-display mb-1">Cyber Punk Jacket</h3>
                              <p class="text-cyan-400 font-bold text-xl">$1,299.00</p>
                          </div>
                      </div>
                 </div>

                 <!-- Decorative Elements behind -->
                 <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] border border-white/5 rounded-full z-0"></div>
                 <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] border border-white/5 rounded-full z-0 opacity-50"></div>
            </div>
        </div>
    </div>

    <!-- 2. Trending Categories (Bento Grid) -->
    <section class="py-32 bg-[#050505]">
        <div class="container mx-auto px-6">
             <div class="flex justify-between items-end mb-16" data-aos="fade-up">
                 <div>
                     <h2 class="font-display text-4xl md:text-5xl font-bold mb-4">Danh Mục <span class="text-neon">Trending</span></h2>
                     <p class="text-gray-400 text-lg">Những items được săn đón nhiều nhất</p>
                 </div>
                 <button class="text-white hover:text-cyan-400 transition font-medium flex items-center gap-2">
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
                          <h3 class="text-3xl font-display font-bold text-white group-hover:text-cyan-400 transition-colors">{{ cat.name }}</h3>
                      </div>
                 </div>
             </div>
        </div>
    </section>

    <!-- 3. New Arrivals -->
    <section class="py-32 bg-[#0f1012] relative">
        <div class="container mx-auto px-6 relative z-10">
             <div class="text-center mb-20" data-aos="fade-up">
                 <span class="text-cyan-500 font-bold uppercase tracking-[0.2em] text-sm mb-4 block">New Drops</span>
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
    <section class="py-32 bg-black relative overflow-hidden">
        <!-- Glows -->
        <div class="absolute top-0 left-0 w-[500px] h-[500px] bg-blue-600/10 rounded-full blur-[120px]"></div>
        <div class="absolute bottom-0 right-0 w-[500px] h-[500px] bg-purple-600/10 rounded-full blur-[120px]"></div>
        
        <div class="container mx-auto px-6 relative z-10">
             <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
                 <!-- Feature 1 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-cyan-500/20 group-hover:border-cyan-500/50 transition-colors">
                         <Zap :size="32" class="text-cyan-400" />
                     </div>
                     <h3 class="text-xl font-bold text-white mb-4 font-display">Fast Delivery</h3>
                     <p class="text-gray-400 leading-relaxed">Giao hàng hỏa tốc nội thành trong 2h. Đóng gói cao cấp bảo vệ sản phẩm.</p>
                 </div>

                 <!-- Feature 2 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up" data-aos-delay="100">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-purple-500/20 group-hover:border-purple-500/50 transition-colors">
                         <ShieldCheck :size="32" class="text-purple-400" />
                     </div>
                     <h3 class="text-xl font-bold text-white mb-4 font-display">Premium Quality</h3>
                     <p class="text-gray-400 leading-relaxed">Cam kết chính hãng 100%. Bảo hành đường may trọn đời.</p>
                 </div>

                 <!-- Feature 3 -->
                 <div class="glass p-10 rounded-3xl text-center group hover:-translate-y-2 transition-transform duration-500" data-aos="fade-up" data-aos-delay="200">
                     <div class="w-16 h-16 bg-white/5 rounded-2xl flex items-center justify-center mx-auto mb-8 border border-white/10 group-hover:bg-pink-500/20 group-hover:border-pink-500/50 transition-colors">
                         <ShoppingBag :size="32" class="text-pink-400" />
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
/* Scoped styles mainly handle the float animation delay/specifics if needed */
</style>
