import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

export const useMedalStore = defineStore('medal', () => {
  const medals = ref([]);
  //获取奖牌榜信息
  const getMedals = async () => {
    try {
      const response = await axios.get('/api/dataget/GetAllNationalMedals'); // 替换为实际API
      const { code, message, data } = response.data;

      if (code === 1) {
        medals.value = data;
        console.log(medals.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };

  // 计算总奖牌数
  // const totalMedals = computed(() => {
  //   return medals.value.reduce((total, item) => total + item.total, 0);
  // });
  //
  // // 计算金牌数量
  // const totalGold = computed(() => {
  //   return medals.value.reduce((total, item) => total + item.gold, 0);
  // });

  return {
    medals,
    getMedals,
  };
});
