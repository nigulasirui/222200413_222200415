import { ref } from 'vue';
import { defineStore } from 'pinia';
import axios from '@/plugins/axios';

export const useCompetitionStore = defineStore('competition', () => {
  //类别  男子/女子
  const storeType = localStorage.getItem('types');
  const types = ref(storeType ? JSON.parse(storeType) : []);
  //项目名称
  const storeProject = localStorage.getItem('projects');
  const projects = ref(storeProject ? JSON.parse(storeProject) : []);
  //对阵信息
  const storeInfo = localStorage.getItem('information');
  const information = ref(storeInfo ? JSON.parse(storeInfo) : []);
//"七人制橄榄球",
  const fetchTypes = async () => {
    try {
      const response = await axios.get('/api/dataget/GetAllMatchDetailName',{
        params:{ }
      });
      const { code, message, data } = response.data;

      if (code === 1) {
        types.value = data;
        localStorage.setItem('types', JSON.stringify(types.value));
        console.log(types.value);
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
        console.log(projects.value);
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
  const fetchInfo = async () => {
    try {
      const response = await axios.get('/api/dataget/GetBattleTable');
      const { code, message, data } = response.data;

      if (code === 1) {
        information.value = data;
        localStorage.setItem('information', JSON.stringify(information.value));
        console.log(information.value);
      } else {
        console.error(message);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return {
    types,
    projects,
    information,
    fetchInfo,
    fetchTypes,
    fetchProject
  };
});
