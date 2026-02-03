import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Toast, { type PluginOptions, POSITION } from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import router from './router'
import { MotionPlugin } from '@vueuse/motion'
import AOS from 'aos'
import 'aos/dist/aos.css'
import './style.css'
import App from './App.vue'

// Initialize AOS (Animate On Scroll)
AOS.init({
    duration: 800, // Animation duration (ms)
    easing: 'ease-out-cubic', // Easing function
    once: true, // Only animate once when scrolling down
    offset: 50, // Trigger offset
    delay: 100, // Delay
});

const pinia = createPinia()
const app = createApp(App)

const toastOptions: PluginOptions = {
    position: POSITION.TOP_RIGHT,
    timeout: 3000,
    closeOnClick: true,
    pauseOnFocusLoss: true,
    pauseOnHover: true,
    draggable: true,
    draggablePercent: 0.6,
    showCloseButtonOnHover: false,
    hideProgressBar: false,
    closeButton: "button",
    icon: true,
    rtl: false
};

app.use(pinia)
app.use(router)
app.use(Toast, toastOptions)
app.use(MotionPlugin) // Add Motion Plugin
app.mount('#app')
