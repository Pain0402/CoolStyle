<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { ArrowRight, ShoppingBag, Sparkles } from 'lucide-vue-next';

// State
const products = ref<any[]>([]);
const loading = ref(true);
const error = ref('');

// Static Categories Data for UI Demo (Will be Dynamic later)
const categories = [
    { id: 1, name: 'Thời trang Nam', image: 'https://images.unsplash.com/photo-1516257984-b1b4d8c9230d?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '120+' },
    { id: 2, name: 'Thời trang Nữ', image: 'https://images.unsplash.com/photo-1483985988355-763728e1935b?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '240+' },
    { id: 3, name: 'Phụ kiện', image: 'https://images.unsplash.com/photo-1576053139778-7e32f2ae3cfd?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80', count: '80+' },
];

// Fetch Data
const fetchProducts = async () => {
    try {
        const response: any = await apiClient.get('/products');
        products.value = response as any[]; 
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
    <!-- 1. Hero Section (Premium 3D feel) -->
    <div class="relative bg-slate-900 text-white min-h-[85vh] flex items-center overflow-hidden">
        <!-- Background Overlay -->
        <div class="absolute inset-0 z-0">
             <img src="https://images.unsplash.com/photo-1469334031218-e382a71b716b?ixlib=rb-1.2.1&auto=format&fit=crop&w=2000&q=80" 
                  class="w-full h-full object-cover opacity-40 hover:scale-105 transition duration-[3s]" alt="Hero Background">
        </div>

        <div class="container mx-auto px-6 z-10 grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
            <div data-aos="fade-right" data-aos-duration="1000">
                <span class="inline-block py-1 px-3 rounded-full bg-blue-600/30 border border-blue-500/50 text-blue-300 text-sm font-semibold mb-6 backdrop-blur-sm">
                    🚀 New Collection 2026
                </span>
                <h1 class="text-6xl md:text-8xl font-black leading-tight tracking-tighter mb-6 text-transparent bg-clip-text bg-gradient-to-r from-white to-gray-400">
                    STREET <br> WEAR
                </h1>
                <p class="text-xl text-gray-300 mb-8 max-w-lg font-light leading-relaxed">
                    Khám phá phong cách thời trang đường phố đậm chất riêng. Thiết kế tối giản, chất liệu cao cấp.
                </p>
                <div class="flex gap-4">
                    <button class="btn-primary flex items-center gap-3 bg-white text-black hover:bg-gray-100 hover:text-black shadow-xl shadow-white/10">
                        Mua ngay <ArrowRight :size="20"/>
                    </button>
                    <button class="px-8 py-3.5 border border-white/30 rounded-xl hover:bg-white/10 transition flex items-center gap-2 backdrop-blur-md">
                        Xem Bộ Sưu Tập
                    </button>
                </div>
            </div>

            <!-- Floating Product Card (Motion) -->
            <div class="hidden md:block relative" data-aos="fade-left" data-aos-duration="1200" data-aos-delay="200">
                <div class="relative z-10 bg-white/10 backdrop-blur-lg border border-white/20 p-6 rounded-3xl shadow-2xl transform rotate-3 hover:rotate-0 transition duration-500">
                     <img src="https://images.unsplash.com/photo-1552346154-21d32810aba3?ixlib=rb-1.2.1&auto=format&fit=crop&w=600&q=80" 
                          class="rounded-2xl shadow-lg mb-6 w-full h-[500px] object-cover" alt="Featured Product">
                     <div class="flex justify-between items-end">
                         <div>
                             <p class="text-gray-400 text-sm uppercase tracking-widest mb-1">Featured</p>
                             <h3 class="text-2xl font-bold">Nike Air Max Legacy</h3>
                         </div>
                         <div class="text-right">
                             <p class="text-3xl font-bold text-white">$129.00</p>
                         </div>
                     </div>
                </div>
                <!-- Decorations -->
                <div class="absolute -top-10 -right-10 w-40 h-40 bg-blue-500 rounded-full blur-[100px] opacity-50 z-0"></div>
            </div>
        </div>
        
        <!-- Scroll Indicator -->
        <div class="absolute bottom-10 left-1/2 transform -translate-x-1/2 animate-bounce text-gray-400">
            <span class="text-xs uppercase tracking-widest mb-2 block text-center">Cuộn xuống</span>
            <ArrowRight class="rotate-90 mx-auto" :size="20"/>
        </div>
    </div>

    <!-- 2. Categories Section -->
    <section class="py-24 bg-white relative overflow-hidden">
        <div class="max-w-7xl mx-auto px-4">
             <div class="text-center mb-16" data-aos="fade-up">
                 <h2 class="text-4xl md:text-5xl font-black mb-4">Danh Mục Nổi Bật</h2>
                 <p class="text-gray-500 text-lg">Lựa chọn phong cách phù hợp với bạn</p>
             </div>

             <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
                 <div v-for="(cat, index) in categories" :key="cat.id" 
                      class="group relative h-[450px] overflow-hidden rounded-2xl cursor-pointer"
                      :data-aos="'fade-up'" :data-aos-delay="index * 150">
                     <img :src="cat.image" class="w-full h-full object-cover transition duration-700 group-hover:scale-110" :alt="cat.name">
                     <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition"></div>
                     <div class="absolute bottom-8 left-8 text-white z-10">
                         <span class="block text-sm text-gray-300 mb-2 font-medium">{{ cat.count }} Sản phẩm</span>
                         <h3 class="text-3xl font-bold flex items-center gap-3 group-hover:translate-x-2 transition duration-300">
                             {{ cat.name }} <ArrowRight class="opacity-0 group-hover:opacity-100 transition" :size="24"/>
                         </h3>
                     </div>
                 </div>
             </div>
        </div>
    </section>

    <!-- 3. New Arrivals (Product Grid) -->
    <section class="py-24 bg-slate-50">
        <div class="max-w-7xl mx-auto px-4">
             <div class="flex justify-between items-end mb-12" data-aos="fade-left">
                <div>
                    <span class="text-blue-600 font-bold uppercase tracking-widest text-sm mb-2 block">Mới lên kệ</span>
                    <h2 class="text-4xl font-bold">Sản phẩm mới nhất</h2>
                </div>
                <button class="text-sm font-bold border-b-2 border-black pb-1 hover:text-blue-600 hover:border-blue-600 transition">Xem tất cả</button>
             </div>

             <!-- Loading State -->
             <div v-if="loading" class="grid grid-cols-1 md:grid-cols-4 gap-8">
                <div v-for="i in 4" :key="i" class="h-96 bg-gray-200 animate-pulse rounded-2xl"></div>
             </div>

             <!-- Error -->
             <div v-else-if="error" class="text-center py-12 bg-red-50 rounded-xl text-red-500 border border-red-100">
                {{ error }}
             </div>

             <!-- Products -->
             <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-16">
                 <ProductCard v-for="(product, index) in products" :key="product.id" :product="product" 
                              :data-aos="'fade-up'" :data-aos-delay="index * 100" />
             </div>
        </div>
    </section>

    <!-- 4. Quality Promise -->
    <section class="py-24 bg-black text-white relative overflow-hidden">
        <div class="absolute top-0 right-0 w-96 h-96 bg-blue-600 rounded-full blur-[150px] opacity-20"></div>
        <div class="absolute bottom-0 left-0 w-96 h-96 bg-purple-600 rounded-full blur-[150px] opacity-20"></div>
        
        <div class="max-w-7xl mx-auto px-4 text-center relative z-10">
             <div class="grid grid-cols-1 md:grid-cols-3 gap-12">
                 <div class="p-8 rounded-2xl bg-white/5 border border-white/10 backdrop-blur-sm" data-aos="fade-up">
                     <div class="w-16 h-16 bg-blue-500 rounded-full flex items-center justify-center mx-auto mb-6 shadow-lg shadow-blue-500/30">
                         <Sparkles :size="32" />
                     </div>
                     <h3 class="text-xl font-bold mb-3">Chất lượng cao cấp</h3>
                     <p class="text-gray-400">Vải Cotton Compact 100% nhập khẩu, co giãn 4 chiều và bền màu theo thời gian.</p>
                 </div>
                 <div class="p-8 rounded-2xl bg-white/5 border border-white/10 backdrop-blur-sm" data-aos="fade-up" data-aos-delay="100">
                     <div class="w-16 h-16 bg-green-500 rounded-full flex items-center justify-center mx-auto mb-6 shadow-lg shadow-green-500/30">
                         <ShoppingBag :size="32" />
                     </div>
                     <h3 class="text-xl font-bold mb-3">Đổi trả 30 ngày</h3>
                     <p class="text-gray-400">Không vừa size? Không ưng ý? Đổi trả miễn phí trong vòng 30 ngày.</p>
                 </div>
                 <div class="p-8 rounded-2xl bg-white/5 border border-white/10 backdrop-blur-sm" data-aos="fade-up" data-aos-delay="200">
                     <div class="w-16 h-16 bg-purple-500 rounded-full flex items-center justify-center mx-auto mb-6 shadow-lg shadow-purple-500/30">
                         <Sparkles :size="32" />
                     </div>
                     <h3 class="text-xl font-bold mb-3">Giao hàng hỏa tốc</h3>
                     <p class="text-gray-400">Nhận hàng trong 24h đối với nội thành và 2-3 ngày đối với các tỉnh khác.</p>
                 </div>
             </div>
        </div>
    </section>
  </MainLayout>
</template>

<style scoped>
/* Gradient Text Animation */
.text-gradient-animated {
    background: linear-gradient(to right, #fff, #94a3b8, #fff);
    background-size: 200% auto;
    -webkit-background-clip: text;
    background-clip: text;
    color: transparent;
    animation: shine 3s linear infinite;
}

@keyframes shine {
    to {
        background-position: 200% center;
    }
}
</style>
