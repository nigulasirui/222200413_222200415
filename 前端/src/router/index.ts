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
        await dayResultStore.fetchDayResults(dayResultStore.date)
        next()
      }
    },
    {
      path: '/compete',
      name: 'Compete',
      component: ()=>import('@/views/compete/index.vue'),
      beforeEnter: async (to, from, next) => {
        const competitionStore = useCompetitionStore()
        await competitionStore.fetchProject()
        await competitionStore.fetchTypes(competitionStore.selected.firstname)
        await competitionStore.fetchInfo(competitionStore.selected.type.id)
        next()
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
