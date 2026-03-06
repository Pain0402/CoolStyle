<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { Filter, ChevronDown, SlidersHorizontal, X, ChevronLeft, ChevronRight } from 'lucide-vue-next';

const route = useRoute();
const router = useRouter();
const products = ref<any[]>([]);
const loading = ref(true);
const sortBy = ref('newest');
const selectedCategory = ref('');
const showMobileFilters = ref(false);

// Pagination
const currentPage = ref(1);
const pageSize = ref(20);
const totalCount = ref(0);
const totalPages = ref(0);

// Search with debounce
const searchQuery = ref('');
let searchDebounceTimer: ReturnType<typeof setTimeout> | null = null;

// Price Filter
const minPrice = ref<number | undefined>(undefined);
const maxPrice = ref<number | undefined>(undefined);
const priceMin = ref('');
const priceMax = ref('');

const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const fetchProducts = async () => {
    loading.value = true;
    try {
        const params: Record<string, any> = {
            page: currentPage.value,
            pageSize: pageSize.value,
            sortBy: sortBy.value,
        };
        if (selectedCategory.value) params.category = selectedCategory.value;
        if (minPrice.value !== undefined) params.minPrice = minPrice.value;
        if (maxPrice.value !== undefined) params.maxPrice = maxPrice.value;

        const response: any = await apiClient.get('/products', { params });
        products.value = response.items ?? [];
        totalCount.value = response.totalCount ?? 0;
        totalPages.value = response.totalPages ?? 0;
    } catch (err) {
        console.error(err);
    } finally {
        loading.value = false;
    }
};

// Fetch all categories for sidebar (separate lightweight call)
const allCategories = ref<string[]>([]);
const fetchCategories = async () => {
    try {
        const res: any = await apiClient.get('/categories');
        allCategories.value = (res as any[]).map((c: any) => c.name);
    } catch { /* ignore */ }
};

const availableCategories = computed(() => ['All Products', ...allCategories.value]);

const selectCategory = (cat: string) => {
    selectedCategory.value = cat === 'All Products' ? '' : cat;
    currentPage.value = 1;
    router.push({ query: { ...route.query, category: selectedCategory.value || undefined } });
    fetchProducts();
};

const applyPriceFilter = () => {
    minPrice.value = priceMin.value ? Number(priceMin.value) : undefined;
    maxPrice.value = priceMax.value ? Number(priceMax.value) : undefined;
    currentPage.value = 1;
    fetchProducts();
};

const clearFilters = () => {
    selectedCategory.value = '';
    priceMin.value = '';
    priceMax.value = '';
    minPrice.value = undefined;
    maxPrice.value = undefined;
    sortBy.value = 'newest';
    currentPage.value = 1;
    router.push({ query: {} });
    fetchProducts();
};

const goToPage = (page: number) => {
    if (page < 1 || page > totalPages.value) return;
    currentPage.value = page;
    fetchProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

// Debounced search — waits 400ms after user stops typing
const onSearchInput = (e: Event) => {
    const val = (e.target as HTMLInputElement).value;
    searchQuery.value = val;
    if (searchDebounceTimer) clearTimeout(searchDebounceTimer);
    searchDebounceTimer = setTimeout(() => {
        currentPage.value = 1;
        fetchProducts();
    }, 400);
};

watch(sortBy, () => {
    currentPage.value = 1;
    fetchProducts();
});

watch(() => route.query.category, (newCat) => {
    selectedCategory.value = (newCat as string) || '';
    currentPage.value = 1;
    fetchProducts();
}, { immediate: false });

onMounted(() => {
    if (route.query.category) selectedCategory.value = route.query.category as string;
    fetchCategories();
    fetchProducts();
});
</script>

<template>
  <MainLayout>
    <div class="bg-[#050505] min-h-screen pb-20 font-sans">
        <!-- Header -->
        <div class="pt-12 pb-8 px-6 container mx-auto">
            <h1 class="font-display text-4xl md:text-6xl font-black text-white mb-4 uppercase tracking-tight">
                {{ selectedCategory || 'SHOP ALL' }}
            </h1>
            <p class="text-gray-400 max-w-xl text-lg">Khám phá bộ sưu tập đầy đủ của CoolStyle. Từ Streetwear đến Essentials.</p>
        </div>

        <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-12 gap-10">
            <!-- Sidebar -->
            <div class="hidden lg:block lg:col-span-3 space-y-8">
                <div class="p-6 rounded-2xl sticky top-24 border border-white/10 bg-white/5 backdrop-blur-xl">
                    <div class="flex items-center justify-between mb-6">
                        <div class="flex items-center gap-2 text-white font-bold uppercase tracking-widest text-sm">
                            <Filter :size="16" /> Filters
                        </div>
                        <button @click="clearFilters" class="text-xs text-cyan-400 hover:text-cyan-300 transition">Clear All</button>
                    </div>

                    <!-- Categories -->
                    <div class="mb-8">
                        <h4 class="text-gray-500 text-xs font-bold uppercase mb-4 tracking-wider">Category</h4>
                        <ul class="space-y-3">
                            <li v-for="cat in availableCategories" :key="cat">
                                <button @click="selectCategory(cat)" class="flex items-center gap-3 w-full text-left group">
                                    <div :class="['w-4 h-4 rounded border flex items-center justify-center transition', (cat === 'All Products' ? !selectedCategory : selectedCategory === cat) ? 'bg-cyan-500 border-cyan-500' : 'border-white/20 group-hover:border-cyan-500']">
                                        <div v-if="(cat === 'All Products' ? !selectedCategory : selectedCategory === cat)" class="w-1.5 h-1.5 bg-black rounded-sm"></div>
                                    </div>
                                    <span :class="['text-sm transition', (cat === 'All Products' ? !selectedCategory : selectedCategory === cat) ? 'text-white font-bold' : 'text-gray-400 group-hover:text-white']">{{ cat }}</span>
                                </button>
                            </li>
                        </ul>
                    </div>

                    <!-- Price Filter -->
                    <div>
                        <h4 class="text-gray-500 text-xs font-bold uppercase mb-4 tracking-wider">Price Range</h4>
                        <div class="grid grid-cols-2 gap-2 mb-3">
                            <div>
                                <label class="text-[10px] text-gray-500 uppercase block mb-1">Min</label>
                                <input type="number" v-model="priceMin" placeholder="0" class="w-full bg-white/5 border border-white/10 rounded px-2 py-1 text-xs text-white focus:outline-none focus:border-cyan-500 transition" />
                            </div>
                            <div>
                                <label class="text-[10px] text-gray-500 uppercase block mb-1">Max</label>
                                <input type="number" v-model="priceMax" placeholder="∞" class="w-full bg-white/5 border border-white/10 rounded px-2 py-1 text-xs text-white focus:outline-none focus:border-cyan-500 transition" />
                            </div>
                        </div>
                        <button @click="applyPriceFilter" class="w-full py-2 bg-cyan-500/10 border border-cyan-500/30 text-cyan-400 text-xs font-bold rounded-lg hover:bg-cyan-500/20 transition">
                            Apply
                        </button>
                    </div>
                </div>
            </div>

            <!-- Product Grid -->
            <div class="col-span-1 lg:col-span-9">
                <!-- Toolbar -->
                <div class="flex flex-wrap items-center justify-between mb-8 gap-4 bg-white/5 p-4 rounded-xl border border-white/5 backdrop-blur-sm">
                    <button @click="showMobileFilters = !showMobileFilters" class="lg:hidden flex items-center gap-2 text-sm py-2 px-4 hover:bg-white/10 rounded-lg transition text-white">
                        <SlidersHorizontal :size="16" /> Filters
                    </button>
                    <p class="text-gray-400 text-sm hidden md:block">
                        <span class="text-white font-bold">{{ totalCount }}</span> sản phẩm
                    </p>
                    <div class="relative group ml-auto">
                        <button class="flex items-center gap-2 text-white bg-black/20 border border-white/10 px-4 py-2 rounded-lg hover:border-cyan-500/50 transition text-sm">
                            Sort: <span class="text-cyan-400 font-bold">{{ sortBy === 'newest' ? 'Newest' : sortBy === 'price-asc' ? 'Price ↑' : 'Price ↓' }}</span> <ChevronDown :size="14"/>
                        </button>
                        <div class="absolute right-0 mt-2 w-48 rounded-xl py-2 hidden group-hover:block z-30 border border-white/10 shadow-xl bg-[#0a0a0a]">
                            <button @click="sortBy = 'newest'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300">Newest</button>
                            <button @click="sortBy = 'price-asc'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300">Price: Low → High</button>
                            <button @click="sortBy = 'price-desc'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300">Price: High → Low</button>
                        </div>
                    </div>
                </div>

                <!-- Loading -->
                <div v-if="loading" class="grid grid-cols-2 md:grid-cols-3 gap-6">
                    <div v-for="i in 6" :key="i" class="aspect-[3/4] bg-white/5 rounded-2xl animate-pulse"></div>
                </div>

                <!-- Empty -->
                <div v-else-if="products.length === 0" class="flex flex-col items-center justify-center py-20 text-center">
                    <Filter :size="48" class="text-gray-600 mb-4" />
                    <h3 class="text-xl font-bold text-white mb-2">Không tìm thấy sản phẩm</h3>
                    <button @click="clearFilters" class="mt-4 px-6 py-2 bg-cyan-500 text-black font-bold rounded-lg">Xóa bộ lọc</button>
                </div>

                <!-- Grid -->
                <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-x-6 gap-y-10">
                    <ProductCard v-for="product in products" :key="product.id" :product="product" />
                </div>

                <!-- Pagination -->
                <div v-if="totalPages > 1" class="flex items-center justify-center gap-2 mt-12">
                    <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1"
                            class="w-10 h-10 rounded-xl border border-white/10 flex items-center justify-center text-gray-400 hover:border-cyan-500 hover:text-cyan-400 disabled:opacity-30 disabled:cursor-not-allowed transition">
                        <ChevronLeft :size="18" />
                    </button>
                    <template v-for="p in totalPages" :key="p">
                        <button v-if="p === 1 || p === totalPages || Math.abs(p - currentPage) <= 1"
                                @click="goToPage(p)"
                                :class="['w-10 h-10 rounded-xl border text-sm font-bold transition',
                                         p === currentPage ? 'border-cyan-500 bg-cyan-500/10 text-cyan-400' : 'border-white/10 text-gray-400 hover:border-cyan-500 hover:text-cyan-400']">
                            {{ p }}
                        </button>
                        <span v-else-if="p === 2 && currentPage > 3" class="text-gray-600 px-1">...</span>
                        <span v-else-if="p === totalPages - 1 && currentPage < totalPages - 2" class="text-gray-600 px-1">...</span>
                    </template>
                    <button @click="goToPage(currentPage + 1)" :disabled="currentPage === totalPages"
                            class="w-10 h-10 rounded-xl border border-white/10 flex items-center justify-center text-gray-400 hover:border-cyan-500 hover:text-cyan-400 disabled:opacity-30 disabled:cursor-not-allowed transition">
                        <ChevronRight :size="18" />
                    </button>
                </div>
            </div>
        </div>

        <!-- Mobile Filters -->
        <div v-if="showMobileFilters" class="fixed inset-0 z-50 lg:hidden">
            <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="showMobileFilters = false"></div>
            <div class="absolute right-0 top-0 h-full w-80 bg-[#111] overflow-y-auto p-6 shadow-2xl border-l border-white/10">
                <div class="flex items-center justify-between mb-8">
                    <h3 class="text-white font-bold text-lg uppercase">Filters</h3>
                    <button @click="showMobileFilters = false" class="text-gray-400 hover:text-white"><X :size="24" /></button>
                </div>
                <div class="mb-8">
                    <h4 class="text-gray-500 text-xs font-bold uppercase mb-4">Category</h4>
                    <ul class="space-y-3">
                        <li v-for="cat in availableCategories" :key="cat">
                            <button @click="selectCategory(cat); showMobileFilters = false" class="flex items-center gap-3 w-full text-left">
                                <div :class="['w-4 h-4 rounded border flex items-center justify-center', (cat === 'All Products' ? !selectedCategory : selectedCategory === cat) ? 'bg-cyan-500 border-cyan-500' : 'border-white/20']">
                                    <div v-if="(cat === 'All Products' ? !selectedCategory : selectedCategory === cat)" class="w-1.5 h-1.5 bg-black rounded-sm"></div>
                                </div>
                                <span :class="[(cat === 'All Products' ? !selectedCategory : selectedCategory === cat) ? 'text-white font-bold' : 'text-gray-400']">{{ cat }}</span>
                            </button>
                        </li>
                    </ul>
                </div>
                <div class="pt-4 border-t border-white/10">
                    <button @click="clearFilters(); showMobileFilters = false" class="w-full py-3 border border-white/10 rounded-lg text-white mb-3 font-bold text-sm">Clear All</button>
                    <button @click="showMobileFilters = false" class="w-full py-3 bg-cyan-500 rounded-lg text-black font-bold">View Results</button>
                </div>
            </div>
        </div>
    </div>
  </MainLayout>
</template>
