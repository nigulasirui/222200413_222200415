import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

export const useMedalStore = defineStore('medal', () => {
  const storedMedals = localStorage.getItem('medals');
  const medals = ref(storedMedals ? JSON.parse(storedMedals) : []);
  //获取奖牌榜信息
  const fetchMedals = async () => {
    try {
      const response = await axios.get('/api/dataget/GetAllNationalMedals');
      const { code, message, data } = response.data;

      if (code === 1) {
        medals.value = data;
        localStorage.setItem('medals', JSON.stringify(medals.value));
        // console.log(medals.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return {
    medals,
    fetchMedals,
  };
});
