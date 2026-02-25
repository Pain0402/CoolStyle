<script setup lang="ts">
import { ref, onMounted } from 'vue';
import apiClient from '../utils/api';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';
import { Star } from 'lucide-vue-next';

const props = defineProps<{
    productId: number;
}>();

const authStore = useAuthStore();
const toast = useToast();

const reviews = ref<any[]>([]);
const loading = ref(true);

const newReview = ref({
    rating: 5,
    comment: ''
});

const isSubmitting = ref(false);

const fetchReviews = async () => {
    try {
        loading.value = true;
        const response: any = await apiClient.get(`/products/${props.productId}/reviews`);
        if (response) {
            reviews.value = response;
        }
    } catch (err) {
        console.error("Failed to fetch reviews:", err);
    } finally {
        loading.value = false;
    }
};

const submitReview = async () => {
    if (!authStore.isAuthenticated) {
        toast.error("Vui lòng đăng nhập để đánh giá sản phẩm!");
        return;
    }

    if (!newReview.value.comment.trim()) {
        toast.error("Vui lòng nhập nội dung đánh giá!");
        return;
    }

    try {
        isSubmitting.value = true;
        const response: any = await apiClient.post(`/products/${props.productId}/reviews`, newReview.value);
        if (response) {
            reviews.value.unshift(response);
            toast.success("Đánh giá của bạn đã được gửi!");
            newReview.value.comment = '';
            newReview.value.rating = 5;
        }
    } catch (err) {
        console.error("Failed to submit review:", err);
        toast.error("Không thể gửi bài đánh giá. Vui lòng thử lại!");
    } finally {
        isSubmitting.value = false;
    }
};

onMounted(() => {
    fetchReviews();
});

const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
};
</script>

<template>
    <div class="mt-16 bg-white/5 border border-white/10 rounded-2xl p-6 lg:p-10">
        <h3 class="font-display text-2xl font-bold text-white mb-8 flex items-center gap-3">
            Đánh giá sản phẩm <span class="bg-cyan-500/20 text-cyan-400 text-sm px-3 py-1 rounded-full">{{ reviews.length }}</span>
        </h3>

        <!-- Review Form -->
        <div v-if="authStore.isAuthenticated" class="mb-12 bg-black/40 rounded-xl p-6 border border-white/5">
            <h4 class="text-lg font-bold text-white mb-4">Gửi đánh giá của bạn</h4>
            
            <div class="flex items-center gap-2 mb-4">
                <span class="text-sm text-gray-400 mr-2">Đánh giá chung:</span>
                <button v-for="star in 5" :key="star" @click="newReview.rating = star" class="text-yellow-400 hover:scale-110 transition duration-200">
                    <Star :size="24" :fill="star <= newReview.rating ? 'currentColor' : 'none'" />
                </button>
            </div>

            <textarea 
                v-model="newReview.comment" 
                rows="4" 
                placeholder="Chia sẻ cảm nhận của bạn về sản phẩm này..." 
                class="w-full bg-white/5 border border-white/10 rounded-xl p-4 text-white focus:outline-none focus:border-cyan-500 transition mb-4 resize-none"
            ></textarea>

            <div class="flex justify-end">
                <button 
                    @click="submitReview" 
                    :disabled="isSubmitting"
                    class="bg-cyan-500 text-black px-6 py-2.5 rounded-xl font-bold hover:bg-cyan-400 transition shadow-[0_0_15px_rgba(0,242,234,0.3)] disabled:opacity-50 disabled:cursor-not-allowed">
                    {{ isSubmitting ? 'Đang gửi...' : 'Gửi Đánh Giá' }}
                </button>
            </div>
        </div>
        <div v-else class="mb-12 p-4 bg-white/5 rounded-xl border border-white/10 text-center">
            <p class="text-gray-400 mb-3">Vui lòng đăng nhập để gửi đánh giá cho sản phẩm này.</p>
            <router-link to="/login" class="inline-block text-cyan-400 font-bold hover:underline">Đăng nhập ngay</router-link>
        </div>

        <!-- Reviews List -->
        <div v-if="loading" class="flex justify-center py-10">
             <div class="animate-spin w-8 h-8 border-4 border-cyan-500 border-t-transparent rounded-full"></div>
        </div>
        
        <div v-else-if="reviews.length === 0" class="text-center py-10 text-gray-400">
            Chưa có đánh giá nào. Hãy trở thành người đầu tiên đánh giá sản phẩm này!
        </div>

        <div v-else class="space-y-6">
            <div v-for="review in reviews" :key="review.id" class="border-b border-white/10 pb-6 last:border-0">
                <div class="flex items-start justify-between mb-3">
                    <div class="flex items-center gap-3">
                        <div class="w-10 h-10 rounded-full bg-gradient-to-tr from-cyan-500 to-blue-500 flex items-center justify-center text-black font-bold text-lg">
                            {{ review.userName.charAt(0).toUpperCase() }}
                        </div>
                        <div>
                            <div class="font-bold text-white">{{ review.userName }}</div>
                            <div class="text-xs text-gray-500">{{ formatDate(review.createdAt) }}</div>
                        </div>
                    </div>
                    <div class="flex items-center text-yellow-400">
                        <Star v-for="i in 5" :key="i" :size="14" :fill="i <= review.rating ? 'currentColor' : 'none'" />
                    </div>
                </div>
                <p class="text-gray-300 text-sm leading-relaxed pl-13">
                    {{ review.comment }}
                </p>
            </div>
        </div>
    </div>
</template>
