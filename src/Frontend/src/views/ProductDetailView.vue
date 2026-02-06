<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { useCartStore } from '../stores/cart';
import { useToast } from 'vue-toastification';
import { Star, Truck, ShieldCheck, Heart, Share2, Plus, Minus } from 'lucide-vue-next';

const route = useRoute();
const cartStore = useCartStore();
const toast = useToast();

const product = ref<any>(null);
const loading = ref(true);
const quantity = ref(1);
const activeImage = ref(0);

// Mock images (since backend usually sends one)
const images = ref<string[]>([]);
const relatedProducts = ref<any[]>([]);

const fetchProduct = async () => {
    try {
        // In real app, fetch by slug. For now we might need to search or fetch logic.
        // Assuming API supports /products/:id or we fetch all and find (temporary demo).
        // Let's assume we pass ID via state or fetch list.
        // Quick hack: Fetch all and find by slug (Not performant but works for demo)
        const response: any = await apiClient.get('/products');
        const found = (response as any[]).find(p => p.slug === route.params.slug);
        
        if (found) {
            product.value = found;
            images.value = [
                found.thumbnailUrl || 'https://placehold.co/600x800',
                // Add dupes for gallery demo
                found.thumbnailUrl,
                found.thumbnailUrl
            ];
            
            // Mock related products (exclude current)
            relatedProducts.value = (response as any[])
                .filter(p => p.id !== found.id)
                .slice(0, 4);
        }
    } catch (err) {
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const addToCart = () => {
    if (!product.value) return;
    cartStore.addToCart(product.value); // TODO: Handle quantity in store
    toast.success(`Đã thêm ${quantity.value} sản phẩm vào giỏ!`);
};

onMounted(() => {
    fetchProduct();
});

watch(() => route.params.slug, () => {
    fetchProduct();
    // Reset state if needed
    activeImage.value = 0;
    quantity.value = 1;
});

const formatCurrency = (val: number) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
</script>

<template>
  <MainLayout>
    <div v-if="loading" class="h-screen flex items-center justify-center bg-[#050505]">
        <div class="animate-spin w-12 h-12 border-4 border-cyan-500 border-t-transparent rounded-full"></div>
    </div>

    <div v-else-if="!product" class="h-screen flex items-center justify-center bg-[#050505] text-white">
        Product not found
    </div>

    <div v-else class="bg-[#050505] min-h-screen text-white pt-10 pb-20">
        <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-2 gap-16">
            
            <!-- Left: Gallery (Sticky) -->
            <div class="space-y-6">
                <div class="relative rounded-3xl overflow-hidden glass border border-white/10 aspect-[3/4] group">
                    <img :src="images[activeImage]" class="w-full h-full object-cover transition duration-500 group-hover:scale-105" alt="Product Main">
                    
                    <div class="absolute top-4 right-4 flex flex-col gap-3">
                        <button class="w-10 h-10 rounded-full bg-white/10 backdrop-blur flex items-center justify-center hover:bg-white/20 transition text-white">
                            <Heart :size="20" />
                        </button>
                        <button class="w-10 h-10 rounded-full bg-white/10 backdrop-blur flex items-center justify-center hover:bg-white/20 transition text-white">
                            <Share2 :size="20" />
                        </button>
                    </div>
                </div>

                <!-- Thumbnails -->
                <div class="flex gap-4 overflow-x-auto pb-2">
                    <button v-for="(img, idx) in images" :key="idx" 
                            @click="activeImage = idx"
                            :class="['w-24 h-24 rounded-xl overflow-hidden border-2 transition', activeImage === idx ? 'border-cyan-500' : 'border-transparent opacity-60 hover:opacity-100']">
                        <img :src="img" class="w-full h-full object-cover">
                    </button>
                </div>
            </div>

            <!-- Right: Info -->
            <div class="lg:sticky lg:top-32 h-fit">
                <div class="mb-2 text-cyan-400 font-bold uppercase tracking-widest text-sm">{{ product.categoryName }}</div>
                <h1 class="font-display text-4xl md:text-5xl font-bold mb-4 leading-tight">{{ product.name }}</h1>
                
                <div class="flex items-center gap-4 mb-8">
                    <div class="text-3xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-white to-gray-400">
                        {{ formatCurrency(product.basePrice) }}
                    </div>
                    <div class="flex items-center gap-1 text-yellow-400 text-sm bg-yellow-400/10 px-2 py-1 rounded">
                         <Star :size="14" fill="currentColor" /> 4.8 (120 reviews)
                    </div>
                </div>

                <!-- Divider -->
                <div class="h-px bg-white/10 my-8"></div>

                <!-- Size Selector (Mock) -->
                <div class="mb-8">
                    <div class="flex justify-between mb-3">
                        <span class="font-bold text-sm text-gray-300">Select Size</span>
                        <a href="#" class="text-xs text-cyan-400 underline">Size Chart</a>
                    </div>
                    <div class="flex gap-3">
                        <button v-for="size in ['S', 'M', 'L', 'XL']" :key="size" class="w-12 h-12 rounded-lg border border-white/20 hover:border-cyan-500 hover:bg-cyan-500/10 flex items-center justify-center font-bold transition">
                            {{ size }}
                        </button>
                    </div>
                </div>

                <!-- Actions -->
                <div class="flex gap-6 mb-8">
                    <!-- Qty -->
                    <div class="flex items-center bg-white/5 rounded-xl border border-white/10">
                        <button @click="quantity > 1 ? quantity-- : null" class="w-12 h-12 flex items-center justify-center hover:bg-white/10 text-gray-400 hover:text-white"><Minus :size="16"/></button>
                        <span class="w-8 text-center font-bold">{{ quantity }}</span>
                        <button @click="quantity++" class="w-12 h-12 flex items-center justify-center hover:bg-white/10 text-gray-400 hover:text-white"><Plus :size="16"/></button>
                    </div>
                    
                    <button @click="addToCart" class="btn-primary w-full flex-1 text-center bg-[#00f2ea] text-black hover:bg-[#00c2bb]">
                        Add to Cart - {{ formatCurrency(product.basePrice * quantity) }}
                    </button>
                </div>

                <!-- Features -->
                <div class="space-y-4 text-sm text-gray-400">
                    <div class="flex items-center gap-3">
                        <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center"><Truck :size="16"/></div>
                        <span>Free shipping on orders over 500k</span>
                    </div>
                    <div class="flex items-center gap-3">
                        <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center"><ShieldCheck :size="16"/></div>
                        <span>Authenticity Guaranteed</span>
                    </div>
                </div>
                
                <!-- Description -->
                <div class="mt-8 p-6 rounded-2xl bg-white/5 border border-white/10">
                    <h3 class="font-bold text-white mb-2">Description</h3>
                    <p class="text-gray-400 leading-relaxed text-sm">
                        {{ product.description || 'Chất liệu vải cao cấp, thoáng mát. Thiết kế hiện đại phù hợp với mọi hoạt động hàng ngày.' }}
                    </p>
                </div>
            </div>
        </div>
        
        <!-- Related Products -->
        <div class="container mx-auto px-6 mt-20">
            <h2 class="text-2xl font-display font-bold text-white mb-8 border-l-4 border-cyan-400 pl-4">Bạn có thể thích</h2>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
                 <ProductCard v-for="p in relatedProducts" :key="p.id" :product="p" />
            </div>
        </div>
    </div>
  </MainLayout>
</template>
