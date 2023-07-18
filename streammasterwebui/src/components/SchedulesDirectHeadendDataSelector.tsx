/* eslint-disable @typescript-eslint/no-unused-vars */

import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Toast } from 'primereact/toast';
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";

const SchedulesDirectHeadendDataSelector = (props: SchedulesDirectHeadendDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);
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
    <>
      <Toast position="bottom-right" ref={toast} />

      <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
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
    </>
  );
}

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

type SchedulesDirectHeadendDataSelectorProps = {
  country: string | null;
  // onChange: ((value: string) => void);
  postalCode: string | null;
};

export default React.memo(SchedulesDirectHeadendDataSelector);
