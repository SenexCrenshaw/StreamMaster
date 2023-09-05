

import { useMemo, memo } from "react";
import { useSchedulesDirectGetLineupsQuery } from "../../store/iptvApi";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import SchedulesDirectLineUpPreviewDataSelector from "./SchedulesDirectStationPreviewDataSelector";

const SchedulesDirectLineUpsDataSelector = (props: SchedulesDirectLineUpsDataSelectorProps) => {

  const getLineUpsQuery = useSchedulesDirectGetLineupsQuery();


  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'lineup' },
      { field: 'location' },
      { field: 'name' },
      { field: 'transport' },
      { field: 'isDeleted' },
    ]
  }, []);

  return (
    <div className='m3uFilesEditor flex flex-column border-2 border-round surface-border'>
      {/* <h3><span className='text-bold'>LineUps | </span><span className='text-bold text-blue-500'>{props.country}</span> - <span className='text-bold text-500'>{props.postalCode}</span></h3> */}
      <DataSelector
        columns={sourceColumns}
        dataSource={getLineUpsQuery.data?.lineups}
        emptyMessage="No Streams"


        id='StreamingServerStatusPanel'
        isLoading={getLineUpsQuery.isLoading}
        style={{ height: 'calc(50vh - 40px)' }}
      />
      {/* <SchedulesDirectLineUpPreviewDataSelector lineUps={getLineUpsQuery.data?.lineups} /> */}
    </div>
  );
}

SchedulesDirectLineUpsDataSelector.displayName = 'SchedulesDirectLineUpsDataSelector';

type SchedulesDirectLineUpsDataSelectorProps = {

};

export default memo(SchedulesDirectLineUpsDataSelector);
