import { useSchedulesDirectGetStatusQuery } from '@/lib/iptvApi'
import SchedulesDirectSchedulesDataSelector from '@/src/components/schedulesDirect/SchedulesDirectSchedulesDataSelector'
import { memo, useMemo } from 'react'

const SDEditor = () => {
  const getStatusQuery = useSchedulesDirectGetStatusQuery()

  const status = useMemo(() => {
    if (
      getStatusQuery.data?.systemStatus?.[0].status?.toLocaleLowerCase() ===
      'online'
    ) {
      return (
        <div>
          Schedules Direct System Status:{' '}
          <span className="text-green-500">Online</span>
        </div>
      )
    }

    return (
      <div>
        Schedules Direct System Status:{' '}
        <span className="text-red-500">Offline</span>
      </div>
    )
  }, [getStatusQuery.data])

  return (
    <>
      {status}
      {/* <SchedulesDirectCountrySelector onChange={(e) => setCountry(e)} value={country} />
      <StringEditorBodyTemplate
        onChange={async (e) => {
          console.debug(e);
          setPostalCode(e.toString());
        }}
        value={postalCode}
      />
      <SchedulesDirectHeadendDataSelector country={country} postalCode={postalCode} /> */}
      {/* <SchedulesDirectLineUpsDataSelector /> */}
      <SchedulesDirectSchedulesDataSelector stationIds={[]} />
    </>
  )
}

SDEditor.displayName = 'SDEditor'

// type SDEditorProps = {
//   readonly data?: ChannelGroupDto | undefined;
//   readonly onChange?: ((value: string) => void) | null;
//   readonly value?: string | null;
// };

export default memo(SDEditor)
