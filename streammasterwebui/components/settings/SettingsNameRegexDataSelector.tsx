import React, { type CSSProperties } from 'react';

import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import SettingsNameRegexAddDialog from './SettingsNameRegexAddDialog';
import SettingsNameRegexDeleteDialog from './SettingsNameRegexDeleteDialog';
type RankedString = {
  rank: number;
  value: string;
};

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

  const sourceActionBodyTemplate = React.useCallback(
    (data: RankedString) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <SettingsNameRegexDeleteDialog value={data.value} values={dataSource.map((a) => a.value)} />
      </div>
    ),
    [dataSource],
  );

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        field: 'rank',
        header: 'Rank',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as React.CSSProperties,
      },
      { field: 'value', header: 'Value' },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate,
        field: 'isHidden',
        fieldType: 'isHidden',
        header: 'Actions',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ];
  }, [sourceActionBodyTemplate]);

  return (
    <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
      <div className="flex justify-content-between align-items-center mb-1">
        <span className="m-0 p-0 gap-1" style={{ color: 'var(--orange-color)' }}>
          List of blacklist regexe to match on tvg-name. Stops at first match
        </span>
        <div className="m-0 p-0 flex gap-1">
          <SettingsNameRegexAddDialog values={dataSource.map((a) => a.value)} />
        </div>
      </div>

      <DataSelector
        columns={sourceColumns}
        dataSource={dataSource}
        defaultSortField="rank"
        emptyMessage="No Data"
        id="SettingsNameRegexDataSelector"
        reorderable
        selectedItemsKey="selectSelectedItems"
      />
    </div>
  );
};

SettingsNameRegexDataSelector.displayName = 'SettingsNameRegexDataSelector';

type SettingsNameRegexDataSelectorProps = {
  readonly data: string[] | undefined;
};

export default React.memo(SettingsNameRegexDataSelector);
