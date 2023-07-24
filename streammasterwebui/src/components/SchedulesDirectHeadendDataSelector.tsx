
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";

const SchedulesDirectHeadendDataSelector = (props: SchedulesDirectHeadendDataSelectorProps) => {
  const [country, setCountry] = React.useState<string>('USA');
  const [postalCode, setPostalCode] = React.useState<string>('19082');

  const getHeadendsQuery = StreamMasterApi.useSchedulesDirectGetHeadendsQuery({ country: country, postalCode: postalCode });

  React.useEffect(() => {
    if (props.country !== undefined && props.country !== null && props.country !== '') {
      setCountry(props.country);
    }
  }, [props.country]);

  React.useEffect(() => {
    if (props.postalCode !== undefined && props.postalCode !== null && props.postalCode !== '') {
      setPostalCode(props.postalCode);
    }
  }, [props.postalCode]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { field: 'headend' },
      { field: 'lineup' },
      { field: 'location' },
      { field: 'name' },
      { field: 'transport' },
    ]
  }, []);

  return (
    <div className='m3uFilesEditor flex flex-column border-2 border-round surface-border'>
      <h3><span className='text-bold'>TV Headends | </span><span className='text-bold text-blue-500'>{props.country}</span> - <span className='text-bold text-500'>{props.postalCode}</span></h3>
      <DataSelector
        columns={sourceColumns}
        dataSource={getHeadendsQuery.data}
        emptyMessage="No Streams"
        enableState={false}
        globalSearchEnabled={false}
        id='StreamingServerStatusPanel'
        isLoading={getHeadendsQuery.isLoading}
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </div>
  );
}

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

type SchedulesDirectHeadendDataSelectorProps = {
  country: string | null;
  // onChange: ((value: string) => void);
  postalCode: string | null;
};

export default React.memo(SchedulesDirectHeadendDataSelector);
