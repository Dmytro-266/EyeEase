<template>
  <TransitionRoot
    :show="props.open"
    as="template"
    enter="transition ease-in-out duration-300 transform"
    enterFrom="-translate-x-full"
    enterTo="translate-x-0"
    leave="transition ease-in-out duration-300 transform"
    leaveFrom="translate-x-0"
    leaveTo="-translate-x-full"
  >
    <div
      class="fixed inset-0 z-40 flex w-64 bg-white dark:bg-gray-800 first-line:shadow-sm ring-1 ring-gray-900 ring-opacity-5 dark:ring-gray-700"
    >
      <div class="relative max-w-xs w-full pt-5 pb-4 flex-1 flex flex-col">
        <div class="flex-row px-4 flex items-center">
          <a class="flex-1">
            <img
              :src="`/logo.${$i18n.locale}.svg`"
              width="43"
              height="43"
              class="h-8 w-auto"
            />
          </a>
          <svg
            @click="emit('onOpenChanged', false)"
            xmlns="http://www.w3.org/2000/svg"
            class="cursor-pointer rotate-180 mx-auto h-5 w-5"
            fill="none"
            viewBox="0 0 24 24"
            stroke="#9ba0aa"
            strokeWidth="{2}"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M13 5l7 7-7 7M5 5l7 7-7 7"
            />
          </svg>
        </div>
        <div class="mt-5 flex-1 h-0 overflow-y-auto">
          <nav class="px-2 space-y-1">
            <RouterLink
              :to="item.href!"
              v-for="item in props.navItems"
              :class="[
                'group border-l-4 px-3 py-2 flex items-center text-sm font-medium',
                item.current
                  ? 'bg-teal-50 border-teal-500 text-teal-700 hover:bg-teal-50 hover:text-teal-700 dark:bg-teal-900 dark:text-teal-100'
                  : 'border-transparent text-gray-900 hover:bg-gray-50 hover:text-gray-900 dark:text-gray-100 dark:hover:bg-gray-700 dark:hover:text-white',
              ]"
            >
              <component
                :is="item.icon"
                :class="[
                  item.current
                    ? 'text-teal-500 group-hover:text-teal-500'
                    : 'text-gray-400 group-hover:text-gray-500',
                  'flex-shrink-0 -ml-1 mr-3 h-6 w-6',
                ]"
              />
              <span class="truncate">{{ t(item.textKey) }}</span>
            </RouterLink>
          </nav>
          
          <div class="px-2 mt-4 space-y-1 border-t border-gray-200 dark:border-gray-700 pt-4">
               <!-- Dark Mode -->
               <div class="group border-l-4 border-transparent px-3 py-2 flex items-center justify-between text-sm font-medium text-gray-900 dark:text-gray-100 hover:bg-gray-50 dark:hover:bg-gray-700">
                   <span class="truncate">{{ t('darkMode') }}</span>
                   <Switch v-model="settings.DarkMode" @update:modelValue="toggleDarkMode" :class="[
                       settings.DarkMode ? 'bg-teal-500' : 'bg-gray-200',
                       'ml-4 relative inline-flex flex-shrink-0 h-5 w-9 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500'
                   ]">
                       <span aria-hidden="true" :class="[
                           settings.DarkMode ? 'translate-x-4' : 'translate-x-0',
                           'inline-block h-4 w-4 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
                       ]" />
                   </Switch>
               </div>

               <!-- Run When Starts -->
               <div class="group border-l-4 border-transparent px-3 py-2 flex items-center justify-between text-sm font-medium text-gray-900 dark:text-gray-100 hover:bg-gray-50 dark:hover:bg-gray-700">
                   <span class="truncate">{{ t('runWhenStarts') }}</span>
                   <Switch v-model="settings.RunWhenStarts" @update:modelValue="saveSettings" :class="[
                       settings.RunWhenStarts ? 'bg-teal-500' : 'bg-gray-200',
                       'ml-4 relative inline-flex flex-shrink-0 h-5 w-9 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500'
                   ]">
                       <span aria-hidden="true" :class="[
                           settings.RunWhenStarts ? 'translate-x-4' : 'translate-x-0',
                           'inline-block h-4 w-4 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
                       ]" />
                   </Switch>
               </div>

               <!-- Reset After Click (ResetWhenSessionUnlock) -->
               <div class="group border-l-4 border-transparent px-3 py-2 flex items-center justify-between text-sm font-medium text-gray-900 dark:text-gray-100 hover:bg-gray-50 dark:hover:bg-gray-700">
                   <span class="truncate">{{ t('resetWhenSessionUnlock') }}</span>
                    <Switch v-model="settings.ResetWhenSessionUnlock" @update:modelValue="saveSettings" :class="[
                       settings.ResetWhenSessionUnlock ? 'bg-teal-500' : 'bg-gray-200',
                       'ml-4 relative inline-flex flex-shrink-0 h-5 w-9 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500'
                   ]">
                       <span aria-hidden="true" :class="[
                           settings.ResetWhenSessionUnlock ? 'translate-x-4' : 'translate-x-0',
                           'inline-block h-4 w-4 rounded-full bg-white shadow transform ring-0 transition ease-in-out duration-200'
                       ]" />
                   </Switch>
               </div>
               
               <!-- Action Buttons -->
               <div class="flex flex-wrap gap-2 mt-4">
                   <button @click="pauseToggle" class="flex-1 group border border-transparent px-3 py-2 flex items-center justify-center text-sm font-medium rounded-md text-white bg-amber-500 hover:bg-amber-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-amber-500">
                        <PauseIcon class="flex-shrink-0 -ml-1 mr-1 h-4 w-4" aria-hidden="true" />
                        {{ t('pause') }}
                   </button>
                   <button @click="reset" class="flex-1 group border border-transparent px-3 py-2 flex items-center justify-center text-sm font-medium rounded-md text-white bg-red-500 hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">
                        <RefreshIcon class="flex-shrink-0 -ml-1 mr-1 h-4 w-4" aria-hidden="true" />
                        {{ t('reset') }}
                   </button>
               </div>
               <button @click="restNow" class="w-full mt-2 group border border-transparent px-4 py-2 flex items-center justify-center text-sm font-medium rounded-md text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500">
                    <PlayIcon class="flex-shrink-0 -ml-1 mr-2 h-5 w-5" aria-hidden="true" />
                    {{ t('rest_now') }}
               </button>
          </div>
        </div>
        <div class="flex-shrink-0 border-t border-gray-200 dark:border-gray-700 p-2">
          <nav>
            <RouterLink
              :to="item.href!"
              v-for="item in props.bottomItems"
              :class="[
                item.current
                  ? 'bg-teal-50 border-teal-500 text-teal-700 hover:bg-teal-50 hover:text-teal-700 dark:bg-teal-900 dark:text-teal-100'
                  : 'border-transparent text-gray-900 hover:bg-gray-50 hover:text-gray-900 dark:text-gray-100 dark:hover:bg-gray-700 dark:hover:text-white',
                'group border-l-4 px-3 py-2 flex items-center text-sm font-medium',
              ]"
            >
              <component
                :is="item.icon"
                :class="[
                  item.current
                    ? 'text-teal-500 group-hover:text-teal-500'
                    : 'text-gray-400 group-hover:text-gray-500',
                  'flex-shrink-0 -ml-1 mr-3 h-6 w-6',
                ]"
              />
              <p class="truncate">{{ t(item.textKey) }}</p>
            </RouterLink>
          </nav>
        </div>
      </div>
    </div>
  </TransitionRoot>
  <Toggle :show="!props.open" @click="emit('onOpenChanged', true)"></Toggle>
</template>
<script lang="ts" setup>
import { TransitionRoot, Switch } from "@headlessui/vue";
import { useI18n } from "vue-i18n";
import type { NavItemModel } from "../models/NavItemModel";
import { RouterLink } from "vue-router";
import Toggle from "./Toggle.vue";
import { useSettingsStore } from "@/stores/settings";
import { storeToRefs } from "pinia";
import { onMounted } from "vue";
import { PlayIcon, PauseIcon, RefreshIcon } from "@heroicons/vue/outline";
import * as client from "@/lib/client";

type PropsType = {
  navItems: NavItemModel[];
  bottomItems: NavItemModel[];
  open: boolean | undefined;
};

type EmitsType = {
  (e: "onOpenChanged", value: boolean): void;
};

const props = defineProps<PropsType>();
const emit = defineEmits<EmitsType>();
const { t } = useI18n();
const store = useSettingsStore();
const { settings } = storeToRefs(store);

const toggleDarkMode = (val: boolean) => {
  store.toggleDarkMode(val);
};

const saveSettings = () => {
  store.saveSettings();
};

const restNow = async () => {
    await client.RestNow();
};

const pauseToggle = async () => {
    await client.Pause();
};

const reset = async () => {
    await client.Reset();
};

onMounted(async () => {
    // Ensure settings are loaded if not already
    if (Object.keys(settings.value).length === 0) {
        await store.fetchSettings();
    }
});
</script>
