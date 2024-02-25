import { FfmpegProfileDto, FfmpegProfileDtos } from '@lib/iptvApi';
import React from 'react';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import ProfileNameEditor from './ProfileNameEditor';

interface SettingsProfilesDataSelectorProperties {
  readonly data: FfmpegProfileDtos | undefined;
}

const SettingsProfilesDataSelector = (props: SettingsProfilesDataSelectorProperties) => {
  const nameEditor = (data: FfmpegProfileDto) => {
    return <ProfileNameEditor data={data} />;
  };

  const sourceActionBodyTemplate = React.useCallback((data: FfmpegProfileDto) => <div className="flex p-0 justify-content-end align-items-center"></div>, []);

  const sourceColumns = React.useMemo(
    (): ColumnMeta[] => [
      {
        field: 'name',
        width: '4rem',
        bodyTemplate: nameEditor
      },

      {
        field: 'parameters',
        width: '10rem'
      },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate,
        field: 'isHidden',
        fieldType: 'isHidden',
        header: 'Actions',
        style: {
          width: '4rem'
        } as CSSProperties
      }
    ],
    [sourceActionBodyTemplate]
  );

  return (
    <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
      <div className="flex justify-content-between align-items-center mb-1">
        <span className="m-0 p-0 gap-1" style={{ color: 'var(--orange-color)' }}>
          List of Profiles
        </span>
        {/* <div className="m-0 p-0 flex gap-1">
          <SettingsNameRegexAddDialog values={dataSource.map((a) => a.value)} />
        </div> */}
      </div>

      <DataSelector
        columns={sourceColumns}
        dataSource={props.data}
        defaultSortField="rank"
        emptyMessage="No Data"
        id="SettingsProfilesDataSelector"
        selectedItemsKey="selectSelectedItems"
      />
    </div>
  );
};

SettingsProfilesDataSelector.displayName = 'SettingsProfilesDataSelector';

export default React.memo(SettingsProfilesDataSelector);
