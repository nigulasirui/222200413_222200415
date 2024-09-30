import { computed, ref } from 'vue'
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

interface Competitor {
  countryEN: string;
  name: string;
  score: string;
  isWinner: boolean;
}

interface Result {
  startDate: string;
  competitor1: Competitor;
  competitor2: Competitor;
}

interface State {
  stateName: string;
  results: Result[];
}

const matchData = {
  startDate: "2024-08-08T17:00",
  competitor1: {
    countryEN: "EGY",
    name: "Egypt",
    score: "0",
    isWinner: false
  },
  competitor2: {
    countryEN: "MAR",
    name: "Morocco",
    score: "6",
    isWinner: true
  }
};

interface Status{
  code:number,
  message:string,
}

export const useCompeteDetailStore = defineStore('competeDetail', () => {
  //项目全部组别数据
  const storeCompeteDetail = localStorage.getItem('competeDetail');
  const competeDetail = ref<State[]>(storeCompeteDetail ? JSON.parse(storeCompeteDetail) : [{ stateName:'',results:[matchData]}]);
  //所选组（默认第一组）
  const defaultResult = computed(()=>competeDetail.value[0].results)
  const selectResult = ref<Result[]>(defaultResult.value?defaultResult.value:[matchData])
  //当前所选组数据（默认第一项）
  const defaultData = computed(()=>selectResult.value[0])
  const selected = ref<Result>(defaultData.value ? defaultData.value : matchData)
  //获取全部组别数据
  const fetchCompeteDetail = async (disciplineCode:string,eventId:string):Promise<Status> => {
    try {
      const response = await axios.get(`api/dataget/GetResultCombine`,{
        params:{ disciplineCode,eventId }
      });

      const { code, message, data } = response.data;

      if (code === 1) {
        competeDetail.value = data;
        localStorage.setItem('competeDetail', JSON.stringify(competeDetail.value));
        // console.log(competeDetail.value);
      }
      return { code, message };
    } catch (error) {
      console.error(error);
      return { code: 0, message: '请求失败' };
    }
  };

  return {
    selectResult,
    selected,
    competeDetail,
    fetchCompeteDetail,
  };
});
