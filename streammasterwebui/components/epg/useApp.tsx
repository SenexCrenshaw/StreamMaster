import { useStreamGroupsGetStreamGroupEpgForGuideQuery } from '@/lib/iptvApi'
import { useEpg, type Channel, type Program } from 'planby'
import React from 'react'

export function useApp(streamGroupNumber: number) {
  const [channels, setChannels] = React.useState<Channel[]>([])
  const [epg, setEpg] = React.useState<Program[]>([])
  const [isLoading, setIsLoading] = React.useState<boolean>(false)

  const epgForGuide =
    useStreamGroupsGetStreamGroupEpgForGuideQuery(streamGroupNumber)

  const channelsData = React.useMemo(() => channels, [channels])
  const epgData = React.useMemo(() => epg, [epg])

  const startDate = React.useMemo(() => {
    const sd = new Date()

    sd.setMinutes(sd.getMinutes() - 30)

    return sd
  }, [])

  const endDate = React.useMemo(() => {
    const sd = new Date()

    sd.setHours(sd.getHours() + 2)

    return sd
  }, [])

  React.useEffect(() => {
    if (epgForGuide.data) {
      setIsLoading(true)
      const chs = epgForGuide.data?.channels as Channel[]

      setChannels(chs)

      const sd = new Date()

      sd.setHours(sd.getHours() - 24)

      var ed = new Date()

      ed.setHours(ed.getHours() + 48)

      const ret = epgForGuide.data?.programs as Program[]
      const test = ret.filter(
        (p) => new Date(p.since) >= sd && new Date(p.since) <= ed,
      )

      setEpg(test)
      setIsLoading(false)
    }
  }, [epgForGuide.data])

  const { getEpgProps, getLayoutProps } = useEpg({
    channels: channelsData,
    dayWidth: 1160,
    endDate: endDate,
    epg: epgData,
    isBaseTimeFormat: true,
    isLine: true,
    isSidebar: true,
    isTimeline: true,
    itemHeight: 80,
    sidebarWidth: 80,
    startDate: startDate,
  })

  return { getEpgProps, getLayoutProps, isLoading }
}
