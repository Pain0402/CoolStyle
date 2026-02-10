<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { useCartStore } from '../stores/cart';
import { useWishlistStore } from '../stores/wishlist';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';
import { Star, Truck, ShieldCheck, Heart, Share2, Plus, Minus, ShoppingBag } from 'lucide-vue-next';

const route = useRoute();
const cartStore = useCartStore();
const wishlistStore = useWishlistStore();
const authStore = useAuthStore();
const toast = useToast();

const product = ref<any>(null);
const loading = ref(true);
const quantity = ref(1);
const activeImage = ref(0);

// Variant State
const selectedColor = ref<string>('');
const selectedSize = ref<string>('');

// Mock images (since backend usually sends one)
const images = ref<string[]>([]);
const relatedProducts = ref<any[]>([]);

// Computed Variants Logic
const uniqueColors = computed(() => {
    if (!product.value?.variants) return [];
    const colors = new Map();
    product.value.variants.forEach((v: any) => {
        if (!colors.has(v.colorName)) {
            colors.set(v.colorName, { name: v.colorName, hex: v.colorHex });
        }
    });
    return Array.from(colors.values());
});

const availableSizes = computed(() => {
    if (!product.value?.variants || !selectedColor.value) return [];
    return product.value.variants
        .filter((v: any) => v.colorName === selectedColor.value)
        .map((v: any) => v.size)
        // Sort sizes: S, M, L, XL...
        .sort((a: string, b: string) => {
            const order = ['S', 'M', 'L', 'XL', '2XL'];
            return order.indexOf(a) - order.indexOf(b);
        });
});

const selectColor = (color: string) => {
    selectedColor.value = color;
    // Auto-select first available size for this color if current size invalid
    const sizes = product.value.variants.filter((v: any) => v.colorName === color).map((v: any) => v.size);
    if (!sizes.includes(selectedSize.value)) {
        selectedSize.value = sizes[0] || '';
    }
};

// Update price based on variant (if needed)
const currentPrice = computed(() => {
    if (!product.value) return 0;
    const variant = product.value.variants?.find((v: any) => 
        v.colorName === selectedColor.value && v.size === selectedSize.value
    );
    return variant ? variant.price : product.value.basePrice;
});

const fetchProduct = async () => {
    try {
        loading.value = true;
        
        // Correct API Call: Get Single Product with Variants
        const response: any = await apiClient.get(`/products/${route.params.slug}`);
        
        if (response) {
            product.value = response;
            
            // Setup Images
            images.value = [
                response.thumbnailUrl || 'https://placehold.co/600x800',
                ...(response.images?.map((img: any) => img.url) || [])
            ];
            
            // Remove duplicates and ensure at least one image
            images.value = [...new Set(images.value)];
            if (images.value.length === 0) images.value = ['https://placehold.co/600x800'];

            // Set Default Variant
            if (response.variants && response.variants.length > 0) {
                const firstVar = response.variants[0];
                selectedColor.value = firstVar.colorName;
                selectedSize.value = firstVar.size;
            }

            // Fetch related products separately if needed or mock for now
            // For now, mocking related to keep it simple, or call list API
            relatedProducts.value = []; 
            fetchRelated(response.categoryName);
        }
    } catch (err) {
        console.error("Failed to fetch product detail:", err);
        product.value = null;
    } finally {
        loading.value = false;
    }
};

const isWishlisted = computed(() => product.value ? wishlistStore.isInWishlist(product.value.id) : false);

const fetchRelated = async (_category: string) => {
    try {
       // Fetch a few products for related section
       // Can optimize later with category ID
       const res: any = await apiClient.get('/products');
       if (Array.isArray(res)) {
           relatedProducts.value = res.filter((p: any) => p.slug !== route.params.slug).slice(0, 4);
       }
    } catch (e) { console.error(e); }
};

const addToCart = () => {
    if (!product.value) return;

    // Find full variant object
    const variant = product.value.variants?.find((v: any) => 
        v.colorName === selectedColor.value && v.size === selectedSize.value
    );

    cartStore.addToCart(product.value, quantity.value, variant);
    toast.success(`Đã thêm ${quantity.value} sản phẩm vào giỏ!`);
};

const addToWishlist = async () => {
    if (!product.value) return;
    
    const added = await wishlistStore.toggleWishlist(product.value);
    if (!authStore.isAuthenticated) {
        toast.error("Vui lòng đăng nhập để sử dụng tính năng này!");
        return;
    }
    
    if (added) {
        toast.success("Đã thêm vào danh sách yêu thích!");
    } else {
        toast.info("Đã xóa khỏi danh sách yêu thích");
    }
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
                        {{ formatCurrency(currentPrice) }}
                    </div>
                    <div class="flex items-center gap-1 text-yellow-400 text-sm bg-yellow-400/10 px-2 py-1 rounded">
                         <Star :size="14" fill="currentColor" /> 4.8 (120 reviews)
                    </div>
                </div>

                <!-- Divider -->
                <div class="h-px bg-white/10 my-8"></div>

                <!-- Variant Selector -->
                <div v-if="product.variants && product.variants.length > 0" class="mb-8 space-y-6">
                    
                    <!-- Color Selection -->
                    <div>
                        <span class="block font-bold text-sm text-gray-300 mb-3">Màu sắc: <span class="text-cyan-400">{{ selectedColor }}</span></span>
                        <div class="flex flex-wrap gap-3">
                            <button v-for="color in uniqueColors" :key="color.name" 
                                    @click="selectColor(color.name)"
                                    :class="['w-10 h-10 rounded-full border-2 flex items-center justify-center transition', selectedColor === color.name ? 'border-cyan-400 scale-110' : 'border-transparent hover:scale-105 ring-1 ring-white/10']"
                                    :style="{ backgroundColor: color.hex }"
                                    :title="color.name">
                                <span v-if="selectedColor === color.name" class="block w-2 h-2 bg-white rounded-full shadow-md"></span>
                            </button>
                        </div>
                    </div>

                    <!-- Size Selection -->
                    <div>
                        <div class="flex justify-between mb-3">
                            <span class="font-bold text-sm text-gray-300">Kích thước: <span class="text-cyan-400">{{ selectedSize }}</span></span>
                            <a href="#" class="text-xs text-cyan-400 underline hover:text-white transition">Hướng dẫn chọn size</a>
                        </div>
                        <div class="flex flex-wrap gap-3">
                            <button v-for="size in availableSizes" :key="size" 
                                    @click="selectedSize = size"
                                    :class="['min-w-[3rem] h-12 px-4 rounded-xl border font-bold transition flex items-center justify-center', 
                                             selectedSize === size ? 'border-cyan-400 bg-cyan-400/10 text-cyan-400 shadow-[0_0_10px_rgba(0,242,234,0.3)]' : 'border-white/10 bg-white/5 hover:border-white/30 text-gray-300']">
                                {{ size }}
                            </button>
                        </div>
                    </div>
                </div>

                <div v-else class="mb-8 p-4 bg-yellow-500/10 border border-yellow-500/20 rounded-xl text-yellow-200 text-sm">
                    Sản phẩm này hiện đang hết hàng hoặc chưa có biến thể.
                </div>

                    <div class="flex gap-4 mb-8">
                        <!-- Qty -->
                        <div class="flex items-center bg-white/5 rounded-xl border border-white/10 h-14">
                            <button @click="quantity > 1 ? quantity-- : null" class="w-12 h-full flex items-center justify-center hover:bg-white/10 text-gray-400 hover:text-white"><Minus :size="16"/></button>
                            <span class="w-8 text-center font-bold">{{ quantity }}</span>
                            <button @click="quantity++" class="w-12 h-full flex items-center justify-center hover:bg-white/10 text-gray-400 hover:text-white"><Plus :size="16"/></button>
                        </div>
                        
                        <button @click="addToCart" class="flex-1 h-14 bg-[#00f2ea] text-black font-bold rounded-xl hover:bg-[#00c2bb] shadow-[0_0_20px_rgba(0,242,234,0.3)] transition-all flex items-center justify-center gap-2">
                            <ShoppingBag :size="20" />
                            <span>Add to Cart - {{ formatCurrency(currentPrice * quantity) }}</span>
                        </button>

                        <button @click="addToWishlist" 
                                :class="['w-14 h-14 rounded-xl border flex items-center justify-center transition-all group', 
                                         isWishlisted ? 'border-red-500 bg-red-500/10 text-red-500' : 'border-white/10 text-white hover:border-red-500 hover:text-red-500 hover:bg-red-500/10']">
                            <Heart :size="24" :fill="isWishlisted ? 'currentColor' : 'none'" class="group-active:scale-90 transition-transform" />
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
