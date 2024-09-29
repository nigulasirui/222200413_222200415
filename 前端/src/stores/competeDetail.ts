import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

interface Competitor {
  countryEN: string;
  name: string;
  score: string;
  isWinner: boolean;
}

interface Result {
  startDate: string; // ISO 8601 date string
  competitor1: Competitor;
  competitor2: Competitor;
}

interface State {
  stateName: string;
  results: Result[];
}

interface ApiResponse {
  code: number;
  message: string | null;
  data: State[];
}


export const useCompeteDetailStore = defineStore('competeDetail', () => {
  const storeCompeteDetail = localStorage.getItem('competeDetail');
  const competeDetail = ref(storeCompeteDetail ? JSON.parse(storeCompeteDetail) : []);
  //
  const fetchCompeteDetail = async (disciplineCode:string,eventId:string) => {
    try {
      const response = await axios.get(`api/dataget/GetResultCombine`,{
        params:{ disciplineCode,eventId }
      });

      const { code, message, data } = response.data;

      if (code === 1) {
        competeDetail.value = data;
        localStorage.setItem('competeDetail', JSON.stringify(competeDetail.value));
        console.log(competeDetail.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return {
    competeDetail,
    fetchCompeteDetail,
  };
});
