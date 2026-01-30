import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Setting } from '@/lib/models';
import * as client from '@/lib/client';
import debounce from 'lodash/debounce';

export const useSettingsStore = defineStore('settings', () => {
    const settings = ref<Setting>({});
    const loading = ref(false);

    const fetchSettings = async () => {
        try {
            loading.value = true;
            const data = await client.getSettings();
            Object.assign(settings.value, data);

            // Sync dark mode state with DOM
            if (settings.value.DarkMode) {
                document.documentElement.classList.add('dark');
            } else {
                document.documentElement.classList.remove('dark');
            }
        } catch (error) {
            console.error('Failed to fetch settings:', error);
        } finally {
            loading.value = false;
        }
    };

    const saveSettings = debounce(async () => {
        try {
            loading.value = true;
            console.log("Saving settings to backend...", settings.value);
            const newSettings = await client.SetSettings(settings.value);
            Object.assign(settings.value, newSettings);
        } catch (error) {
            console.error('Failed to save settings:', error);
        } finally {
            loading.value = false;
        }
    }, 500);

    const toggleDarkMode = (value: boolean) => {
        settings.value.DarkMode = value;
        if (value) {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
        saveSettings();
    };

    return {
        settings,
        loading,
        fetchSettings,
        saveSettings,
        toggleDarkMode
    };
});
