<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import { useAuthStore } from '../stores/auth';
import apiClient from '../utils/api';
import { User, Package, MapPin, Heart, LogOut, Plus, Trash2, CheckCircle } from 'lucide-vue-next';
import { useToast } from 'vue-toastification';
import ProductCard from '../components/ProductCard.vue';

const authStore = useAuthStore();
const toast = useToast();
const activeTab = ref('profile'); // profile, orders, addresses, wishlist
const loading = ref(false);

// Data
const profile = ref<any>({});
const addresses = ref<any[]>([]);
const wishlist = ref<any[]>([]);

// Forms
const showAddressForm = ref(false);
const newAddress = ref({
    recipientName: '',
    phone: '',
    street: '',
    city: '',
    district: '',
    ward: '',
    isDefault: false
});

const fetchProfileData = async () => {
    try {
        loading.value = true;
        
        // Parallel requests
        const [profileRes, addressRes, wishlistRes]: any[] = await Promise.all([
             apiClient.get('/user/profile'),
             apiClient.get('/user/addresses'),
             apiClient.get('/wishlist')
        ]);
        
        profile.value = profileRes;
        addresses.value = addressRes as any[];
        wishlist.value = wishlistRes as any[];

    } catch (e) {
        console.error(e);
        toast.error("Failed to load profile data");
    } finally {
        loading.value = false;
    }
};

const updateProfile = async () => {
    try {
        await apiClient.put('/user/profile', {
            fullName: profile.value.fullName,
            phone: profile.value.phone
        });
        toast.success("Profile updated successfully!");
    } catch (e) {
        toast.error("Failed to update profile");
    }
};

const createAddress = async () => {
    try {
        await apiClient.post('/user/addresses', newAddress.value);
        toast.success("Address added!");
        showAddressForm.value = false;
        // Refresh
        const res: any = await apiClient.get('/user/addresses');
        addresses.value = res as any[];
        // Reset form
        newAddress.value = { recipientName: '', phone: '', street: '', city: '', district: '', ward: '', isDefault: false };
    } catch (e) {
        toast.error("Failed to add address");
    }
};

const deleteAddress = async (id: number) => {
    if(!confirm("Are you sure?")) return;
    try {
        await apiClient.delete(`/user/addresses/${id}`);
        addresses.value = addresses.value.filter(a => a.id !== id);
        toast.success("Address deleted");
    } catch (e) {
        toast.error("Failed to delete address");
    }
};

const setDefaultAddress = async (id: number) => {
    try {
        await apiClient.put(`/user/addresses/${id}/default`);
        // Refresh to update UI (lazy way) or update local state
        const res: any = await apiClient.get('/user/addresses');
        addresses.value = res as any[];
        toast.success("Default address updated");
    } catch (e) {
        toast.error("Failed to set default");
    }
};

const removeFromWishlist = async (productId: number) => {
    try {
        await apiClient.delete(`/wishlist/${productId}`);
        wishlist.value = wishlist.value.filter(w => w.productId !== productId);
        toast.success("Removed from wishlist");
    } catch (e) {
        toast.error("Failed to remove");
    }
};

onMounted(() => {
    if (authStore.isAuthenticated) {
        fetchProfileData();
    }
});

const logout = () => {
    authStore.logout();
    window.location.href = '/';
};
</script>

<template>
  <MainLayout>
    <div class="bg-[#050505] min-h-screen pt-10 pb-20 text-white">
        <div class="container mx-auto px-6">
            
            <div class="flex flex-col md:flex-row gap-10">
                
                <!-- Sidebar -->
                <aside class="w-full md:w-64 space-y-2">
                    <div class="p-6 rounded-2xl glass border border-white/10 mb-6 text-center">
                        <div class="w-24 h-24 rounded-full border-2 border-white/10 shadow-2xl mx-auto mb-4 p-0.5 group">
                            <div class="w-full h-full rounded-full overflow-hidden">
                                <img v-if="profile.avatarUrl" :src="profile.avatarUrl" alt="Avatar" class="w-full h-full object-cover" />
                                <div v-else class="w-full h-full bg-gradient-to-br from-cyan-400 to-blue-600 flex items-center justify-center text-3xl font-bold text-black">
                                    {{ profile.fullName?.charAt(0) }}
                                </div>
                            </div>
                        </div>
                        <h2 class="font-bold truncate text-lg">{{ profile.fullName }}</h2>
                        <p class="text-xs text-gray-400 truncate">{{ profile.email }}</p>
                    </div>

                    <button @click="activeTab = 'profile'" :class="['w-full text-left px-6 py-3 rounded-xl flex items-center gap-3 transition', activeTab === 'profile' ? 'bg-white/10 text-cyan-400 border border-cyan-500/30' : 'hover:bg-white/5 text-gray-400']">
                        <User :size="18" /> Protocol ID (Profile)
                    </button>
                    <button @click="activeTab = 'orders'" :class="['w-full text-left px-6 py-3 rounded-xl flex items-center gap-3 transition', activeTab === 'orders' ? 'bg-white/10 text-cyan-400 border border-cyan-500/30' : 'hover:bg-white/5 text-gray-400']">
                        <Package :size="18" /> Mission Logs (Orders)
                    </button>
                    <button @click="activeTab = 'addresses'" :class="['w-full text-left px-6 py-3 rounded-xl flex items-center gap-3 transition', activeTab === 'addresses' ? 'bg-white/10 text-cyan-400 border border-cyan-500/30' : 'hover:bg-white/5 text-gray-400']">
                        <MapPin :size="18" /> Drop Zones (Address)
                    </button>
                    <button @click="activeTab = 'wishlist'" :class="['w-full text-left px-6 py-3 rounded-xl flex items-center gap-3 transition', activeTab === 'wishlist' ? 'bg-white/10 text-cyan-400 border border-cyan-500/30' : 'hover:bg-white/5 text-gray-400']">
                        <Heart :size="18" /> Favorites
                    </button>
                    <div class="h-px bg-white/10 my-2"></div>
                    <button @click="logout" class="w-full text-left px-6 py-3 rounded-xl flex items-center gap-3 transition text-red-500 hover:bg-red-500/10">
                        <LogOut :size="18" /> Abort Session
                    </button>
                </aside>

                <!-- Content -->
                <div class="flex-1 glass rounded-3xl border border-white/10 p-8 min-h-[500px]">
                    
                    <!-- Profile Tab -->
                    <div v-if="activeTab === 'profile'" class="max-w-xl animate-fade-in-up">
                        <h2 class="text-2xl font-bold mb-6 font-display">Identity Protocol</h2>
                        <form @submit.prevent="updateProfile" class="space-y-6">
                            <div>
                                <label class="block text-sm text-gray-400 mb-2">Full Name</label>
                                <input v-model="profile.fullName" type="text" class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 focus:border-cyan-500 focus:outline-none transition">
                            </div>
                            <div>
                                <label class="block text-sm text-gray-400 mb-2">Email (Read-only)</label>
                                <input :value="profile.email" disabled type="email" class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 opacity-50 cursor-not-allowed">
                            </div>
                            <div>
                                <label class="block text-sm text-gray-400 mb-2">Phone Number</label>
                                <input v-model="profile.phone" type="text" class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 focus:border-cyan-500 focus:outline-none transition">
                            </div>
                            <button type="submit" class="btn-primary bg-cyan-500 text-black hover:bg-cyan-400 px-8 py-3 rounded-xl font-bold">
                                Update Identity
                            </button>
                        </form>
                    </div>

                    <!-- Address Tab -->
                    <div v-if="activeTab === 'addresses'" class="animate-fade-in-up">
                        <div class="flex justify-between items-center mb-6">
                            <h2 class="text-2xl font-bold font-display">Drop Zones</h2>
                            <button @click="showAddressForm = !showAddressForm" class="flex items-center gap-2 text-cyan-400 hover:text-white transition">
                                <Plus :size="18" /> Add New
                            </button>
                        </div>

                        <!-- Add Form -->
                        <div v-if="showAddressForm" class="mb-8 p-6 bg-white/5 rounded-2xl border border-white/10">
                            <h3 class="font-bold mb-4">New Coordinate</h3>
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                                <input v-model="newAddress.recipientName" placeholder="Recipient Name" class="input-glass">
                                <input v-model="newAddress.phone" placeholder="Phone Number" class="input-glass">
                                <input v-model="newAddress.street" placeholder="Street Address" class="input-glass md:col-span-2">
                                <input v-model="newAddress.city" placeholder="City" class="input-glass">
                                <input v-model="newAddress.district" placeholder="District" class="input-glass">
                            </div>
                            <div class="flex gap-4">
                                <button @click="createAddress" class="px-6 py-2 bg-cyan-500 text-black rounded-lg font-bold hover:bg-cyan-400">Save</button>
                                <button @click="showAddressForm = false" class="px-6 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20">Cancel</button>
                            </div>
                        </div>

                        <!-- List -->
                        <div class="space-y-4">
                            <div v-for="addr in addresses" :key="addr.id" class="p-4 rounded-xl border border-white/10 flex justify-between items-center group hover:border-cyan-500/30 transition bg-white/5">
                                <div>
                                    <div class="flex items-center gap-3">
                                        <span class="font-bold text-lg">{{ addr.recipientName }}</span>
                                        <span v-if="addr.isDefault" class="text-xs bg-cyan-500/20 text-cyan-400 px-2 py-0.5 rounded border border-cyan-500/30">Default</span>
                                    </div>
                                    <p class="text-sm text-gray-400">{{ addr.phone }}</p>
                                    <p class="text-sm text-gray-300">{{ addr.street }}, {{ addr.ward }}, {{ addr.district }}, {{ addr.city }}</p>
                                </div>
                                <div class="flex gap-3 opacity-0 group-hover:opacity-100 transition">
                                    <button v-if="!addr.isDefault" @click="setDefaultAddress(addr.id)" title="Set Default" class="p-2 hover:bg-white/10 rounded-full text-gray-400 hover:text-white">
                                        <CheckCircle :size="18" />
                                    </button>
                                    <button @click="deleteAddress(addr.id)" title="Delete" class="p-2 hover:bg-red-500/20 rounded-full text-gray-400 hover:text-red-500">
                                        <Trash2 :size="18" />
                                    </button>
                                </div>
                            </div>
                            <div v-if="addresses.length === 0 && !showAddressForm" class="text-center py-10 text-gray-500">
                                No drop zones configured.
                            </div>
                        </div>
                    </div>

                    <!-- Wishlist Tab -->
                    <div v-if="activeTab === 'wishlist'" class="animate-fade-in-up">
                        <h2 class="text-2xl font-bold mb-6 font-display">Favorites Collection</h2>
                        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                            <div v-for="item in wishlist" :key="item.id" class="relative group">
                                <ProductCard :product="item.product" />
                                <button @click="removeFromWishlist(item.productId)" class="absolute top-2 right-2 bg-black/50 p-2 rounded-full text-white hover:text-red-500 hover:bg-black transition z-10">
                                    <Trash2 :size="16" />
                                </button>
                            </div>
                        </div>
                         <div v-if="wishlist.length === 0" class="text-center py-20 text-gray-500">
                            Your collection is empty.
                        </div>
                    </div>

                    <!-- Orders Tab (Placeholder) -->
                    <div v-if="activeTab === 'orders'" class="animate-fade-in-up">
                        <h2 class="text-2xl font-bold mb-6 font-display">Mission Logs</h2>
                        <div class="text-center py-20 text-gray-500 bg-white/5 rounded-2xl">
                             No missions recorded yet.
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
  </MainLayout>
</template>

<style scoped>
.input-glass {
    width: 100%;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 0.75rem;
    padding: 0.75rem 1rem;
    color: white;
    transition: all 0.2s;
}
.input-glass:focus {
    outline: none;
    border-color: #22d3ee;
    background: rgba(255, 255, 255, 0.1);
}
</style>
