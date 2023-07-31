/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */

import React, { CSSProperties } from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Toast } from 'primereact/toast';
import DataSelector from "../features/dataSelector/DataSelector";
import { ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { getTopToolOptions } from "../common/common";
import { Button } from "primereact/button";
import SettingsNameRegexAddDialog from "./SettingsNameRegexAddDialog";
import SettingsNameRegexDeleteDialog from "./SettingsNameRegexDeleteDialog";
type RankedString = {
  rank: number;
  value: string;
}
const SettingsNameRegexDataSelector = (props: SettingsNameRegexDataSelectorProps) => {

  const dataSource = React.useMemo((): RankedString[] => {
    if (!props.data) {
      return [];
    }

    return props.data.map((value, index) => {
      return {
        rank: index,
        value: value,
      };
    });

  }, [props.data]);

  const sourceActionBodyTemplate = React.useCallback((data: RankedString) => (
    <div className='flex p-0 justify-content-end align-items-center'>
      <SettingsNameRegexDeleteDialog value={data.value} values={dataSource.map((a) => a.value)} />
    </div>
  ), [dataSource]);


  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        field: 'rank', header: 'Rank', style: {
          maxWidth: '8rem',
          width: '8rem',
        } as React.CSSProperties,
      },
      { field: 'value', header: 'Value', },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate, field: 'isHidden', fieldType: 'isHidden', header: 'Actions',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [sourceActionBodyTemplate]);



  return (
    <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
      <div className='flex justify-content-between align-items-center mb-1'>
        <span className='m-0 p-0 gap-1' style={{ color: '#FE7600' }}>List of blacklist regexe to match on tvg-name. Stops at first match</span>
        <div className='m-0 p-0 flex gap-1'>

          <SettingsNameRegexAddDialog values={dataSource.map((a) => a.value)} />
        </div>
      </div>

      <DataSelector
        columns={sourceColumns}
        dataSource={dataSource}
        emptyMessage="No Data"
        enableState={false}
        globalSearchEnabled={false}
        id="SettingsNameRegexDataSelector"
        reorderable
        sortField="rank"
      />
    </div>
  );
}

SettingsNameRegexDataSelector.displayName = 'SettingsNameRegexDataSelector';
SettingsNameRegexDataSelector.defaultProps = {
  onChange: null,
  value: null,
};

type SettingsNameRegexDataSelectorProps = {
  data: string[] | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(SettingsNameRegexDataSelector);
