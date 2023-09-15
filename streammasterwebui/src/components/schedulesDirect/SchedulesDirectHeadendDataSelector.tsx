import { memo, useEffect, useMemo, useState } from "react";
import { useSchedulesDirectGetHeadendsQuery } from "../../store/iptvApi";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";


const SchedulesDirectHeadendDataSelector = (props: SchedulesDirectHeadendDataSelectorProps) => {
  const [country, setCountry] = useState<string>('USA');
  const [postalCode, setPostalCode] = useState<string>('19082');

  const getHeadendsQuery = useSchedulesDirectGetHeadendsQuery({ country: country, postalCode: postalCode });

  useEffect(() => {
    if (props.country !== undefined && props.country !== null && props.country !== '') {
      setCountry(props.country);
    }
  }, [props.country]);

  useEffect(() => {
    if (props.postalCode !== undefined && props.postalCode !== null && props.postalCode !== '') {
      setPostalCode(props.postalCode);
    }
  }, [props.postalCode]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
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
        id='StreamingServerStatusPanel'
        isLoading={getHeadendsQuery.isLoading}
        selectedItemsKey='selectSelectedItems'
        style={{ height: 'calc(50vh - 40px)' }}
      />
    </div>
  );
}

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

type SchedulesDirectHeadendDataSelectorProps = {
  readonly country: string | null;
  // onChange: ((value: string) => void);
  readonly postalCode: string | null;
};

export default memo(SchedulesDirectHeadendDataSelector);
