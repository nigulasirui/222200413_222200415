import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

interface Type {
  id:string,
  description:string
}

interface Selected {
  type:Type,
  firstname:string
}

interface Competitor {
  countryEN: string;
  name: string;
  score: string;
  isWinner: boolean;
}

interface Match {
  competitor1: Competitor;
  competitor2: Competitor;
}

interface Final {
  description: string;
  allMatch: Match[];
}

interface Data {
  id: string;
  final: Final;
  final2: Final;
  halfFinal: Final;
  qFinal: Final;
}

interface ApiResponse {
  code: number;
  message: string | null;
  data: Data;
}

export const useCompetitionStore = defineStore('competition', () => {
  //当前选择
  const storeSelected = localStorage.getItem('selected');
  const selected = ref<Selected>(storeSelected ? JSON.parse(storeSelected) : {
    type: { id: "FBLMTEAM11------------------------",description: "男子" },
    firstname:'足球'
  });
  //类别  男子/女子
  const storeType = localStorage.getItem('types');
  const types = ref<Type[]>(storeType ? JSON.parse(storeType) : []);
  //项目名称
  const storeProject = localStorage.getItem('projects');
  const projects = ref(storeProject ? JSON.parse(storeProject) : []);
  //对阵信息
  const storeInfo = localStorage.getItem('information');
  const information = ref<Data>(storeInfo ? JSON.parse(storeInfo) : {});
//"七人制橄榄球",
  const fetchTypes = async (firstname:string) => {
    try {
      const response = await axios.get(`/api/dataget/GetAllMatchDetailName`,{
        params:{ firstname }
      });
      const { code, message, data } = response.data;

      if (code === 1) {
        types.value = data;
        localStorage.setItem('types', JSON.stringify(types.value));
        // console.log(types.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };
//无参
  const fetchProject = async () => {
    try {
      const response = await axios.get('/api/dataget/GetAllMatchName');
      const { code, message, data } = response.data;

      if (code === 1) {
        projects.value = data;
        localStorage.setItem('projects', JSON.stringify(projects.value));
        // console.log(projects.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };
//{
//             "id": "FBLMTEAM11------------------------",参数
//             "description": "男子"
//         },
//         {
//             "id": "FBLWTEAM11------------------------",
//             "description": "女子"
//         }
  const fetchInfo = async (id:string) => {
    try {
      const response = await axios.get(`/api/dataget/GetBattleTable`,{
        params:{ id }
      });
      const { code, message, data } = response.data;

      if (code === 1) {
        information.value = data;
        localStorage.setItem('information', JSON.stringify(information.value));
        // console.log(information.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return {
    selected,
    types,
    projects,
    information,
    fetchInfo,
    fetchTypes,
    fetchProject
  };
});
