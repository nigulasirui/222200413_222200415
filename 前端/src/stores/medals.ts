import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

interface Status{
  code:number,
  message:string,
}

export const useMedalStore = defineStore('medal', () => {
  const storeMedals = localStorage.getItem('medals');
  const medals = ref(storeMedals ? JSON.parse(storeMedals) : []);
  //获取奖牌榜信息
  const fetchMedals = async ():Promise<Status> => {
    try {
      const response = await axios.get('/api/dataget/GetAllNationalMedals');
      const { code, message, data } = response.data;

      if (code === 1) {
        medals.value = data;
        localStorage.setItem('medals', JSON.stringify(medals.value));
        // console.log(medals.value);
      }
      return { code, message };
    } catch (error) {
      console.error(error);
      return { code: 0, message: '请求失败' };
    }
  };

  return {
    medals,
    fetchMedals,
  };
});
