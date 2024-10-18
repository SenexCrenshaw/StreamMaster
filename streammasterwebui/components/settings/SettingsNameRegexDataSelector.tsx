import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import React from 'react';
import ProfileNameRegexAddDialog from './SettingsNameRegexAddDialog';
import SettingsNameRegexDeleteDialog from './SettingsNameRegexDeleteDialog';

interface RankedString {
  rank: number;
  value: string;
}

const SettingsNameRegexDataSelector = (props: SettingsNameRegexDataSelectorProperties) => {
  const dataSource = React.useMemo((): RankedString[] => {
    if (!props.data) {
      return [];
    }

    return props.data.map((value, index) => ({
      rank: index,
      value
    }));
  }, [props.data]);

  const sourceActionBodyTemplate = React.useCallback(
    (data: RankedString) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <SettingsNameRegexDeleteDialog value={data.value} values={dataSource.map((a) => a.value)} />
      </div>
    ),
    [dataSource]
  );

  const sourceColumns = React.useMemo(
    (): ColumnMeta[] => [
      {
        field: 'rank',
        header: 'Rank',
        width: '8rem'
      },
      { field: 'value', header: 'Value' },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate,
        field: 'isHidden',
        fieldType: 'isHidden',
        header: 'Actions',
        width: '8rem'
      }
    ],
    [sourceActionBodyTemplate]
  );

  return (
    <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
      <div className="flex justify-content-between align-items-center mb-1">
        <span className="m-0 p-0 gap-1" style={{ color: 'var(--orange-color)' }}>
          List of blacklist regexe to match on tvg-name. Stops at first match
        </span>
        <div className="m-0 p-0 flex gap-1">
          <ProfileNameRegexAddDialog values={dataSource.map((a) => a.value)} />
        </div>
      </div>

      <SMDataTable
        columns={sourceColumns}
        dataSource={dataSource}
        defaultSortField="Rank"
        emptyMessage="No Data"
        id="SettingsNameRegexDataSelector"
        reorderable
        selectedItemsKey="selectedItems"
      />
    </div>
  );
};

SettingsNameRegexDataSelector.displayName = 'SettingsNameRegexDataSelector';

interface SettingsNameRegexDataSelectorProperties {
  readonly data: string[] | undefined;
}

export default React.memo(SettingsNameRegexDataSelector);
