import axios from '@/plugins/axios';
import { ref } from 'vue';
import { defineStore } from 'pinia'

interface Result {
  mark:string
}

interface Competitor {
  noc: string;
  name: string;
  results: Result;
}

interface Event {
  disciplineName: string;
  eventUnitName: string;
  id: string;
  disciplineCode: string;
  eventId: string;
  startDate: string;
  competitors: Competitor[];
}

export const useDayResultStore = defineStore('dayResult',()=> {
  //选择日期
  const storeDate = localStorage.getItem('date')
  let date= ref(storeDate?storeDate:'2024-07-24')
  // const date= ref('2024-07-24')
  //所选日期赛程
  const storeDayResults = localStorage.getItem('dayResults')
  const dayResults= ref<Event[]>(storeDayResults?JSON.parse(storeDayResults):[])

  const fetchDayResults = async (date:string) => {
    try {
      const response = await axios.get(`/api/dataget/GetDayResult`, {
        params: { date },
      });
      if (response.data.code === 1) {
        dayResults.value = response.data.data;
        localStorage.setItem('dayResults', JSON.stringify(dayResults.value));
      } else {
        console.error(response.data.message);
      }
    } catch (error) {
      console.error('请求失败:', error);
    }
  };

  return {
    date,
    dayResults,
    fetchDayResults,
  };
})
