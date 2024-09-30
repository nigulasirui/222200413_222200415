import { createRouter, createWebHistory } from 'vue-router'
import { useDayResultStore } from '@/stores/dayResult'
import { useCompetitionStore } from '@/stores/competition'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/home',
      redirect: '/',
    },
    {
      path: '/',
      name: 'Home',
      component: ()=>import('@/views/home/index.vue')
    },
    {
      path: '/rankings',
      name: 'Rankings',
      component: ()=>import('@/views/rankings/index.vue')
    },
    {
      path: '/dayResult',
      name: 'DayResult',
      component: ()=>import('@/views/dayResult/index.vue'),
      beforeEnter: async (to, from, next) => {
        const dayResultStore = useDayResultStore()
        const res = await dayResultStore.fetchDayResults(dayResultStore.date)
        if (res.code===1){
          next()
        }
        else {
          alert(res.message)
          next(false)
        }
      }
    },
    {
      path: '/compete',
      name: 'Compete',
      component: ()=>import('@/views/compete/index.vue'),
      beforeEnter: async (to, from, next) => {
        const competitionStore = useCompetitionStore()
        const res1 = await competitionStore.fetchProject()
        if (res1.code===1){
          const res2 = await competitionStore.fetchTypes(competitionStore.selected.firstname)
          if (res2.code===1){
            const res3 = await competitionStore.fetchInfo(competitionStore.selected.type.id)
            if (res3.code===1){
              next()
            }
            else {
              alert(res3.message)
              next(false)
            }
          }
          else {
            alert(res2.message)
            next(false)
          }
        }
        else {
          alert(res1.message)
          next(false)
        }
      }
    },
    {
      path: '/about',
      name: 'About',
      component: ()=>import('@/views/about/index.vue'),
    },
    {
      path: '/detail',
      name: 'Detail',
      component: ()=>import('@/views/detail/index.vue'),
      children:[
        {
          path:'/matchDetail',
          name:'MatchDetail',
          component:()=>import('@/views/detail/matchDetail.vue')
        },
        {
          path:'/playerList',
          name:'PlayerList',
          component:()=>import('@/views/detail/playerList.vue')
        },
      ]
    },
  ]
})

export default router
