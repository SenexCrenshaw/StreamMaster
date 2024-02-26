import { FfmpegProfileDto, FfmpegProfileDtos } from '@lib/iptvApi';
import React from 'react';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import ProfileAddDialog from './ProfileAddDialog';
import ProfileDeleteDialog from './ProfileDeleteDialog';
import ProfileIsM3U8Editor from './ProfileIsM3U8Editor';
import ProfileNameEditor from './ProfileNameEditor';
import ProfileParameterEditor from './ProfileParameterEditor';

interface SettingsProfilesDataSelectorProperties {
  readonly data: FfmpegProfileDtos | undefined;
}

const SettingsProfilesDataSelector = (props: SettingsProfilesDataSelectorProperties) => {
  const nameEditor = React.useCallback((data: FfmpegProfileDto) => {
    return <ProfileNameEditor data={data} />;
  }, []);

  const parameterEditor = React.useCallback((data: FfmpegProfileDto) => {
    return <ProfileParameterEditor data={data} />;
  }, []);

  // const timeoutEditor = React.useCallback((data: FfmpegProfileDto) => {
  //   return <ProfileTimeOutEditor data={data} />;
  // }, []);

  const ism3u8Editor = React.useCallback((data: FfmpegProfileDto) => {
    return <ProfileIsM3U8Editor data={data} />;
  }, []);

  const sourceActionBodyTemplate = React.useCallback((data: FfmpegProfileDto) => {
    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <ProfileDeleteDialog data={data} iconFilled={false} />
      </div>
    );
  }, []);

  const sourceColumns = React.useMemo(
    (): ColumnMeta[] => [
      {
        field: 'name',
        width: '4rem',
        bodyTemplate: nameEditor
      },

      {
        field: 'parameters',
        width: '10rem',
        bodyTemplate: parameterEditor
      },
      // {
      //   field: 'timeout',
      //   width: '6rem',
      //   bodyTemplate: timeoutEditor
      // },
      {
        field: 'isM3U8',
        width: '6rem',
        bodyTemplate: ism3u8Editor
      },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate,
        field: 'isHidden',
        fieldType: 'isHidden',
        header: 'Actions',
        width: '4rem'
      }
    ],
    [ism3u8Editor, nameEditor, parameterEditor, sourceActionBodyTemplate]
  );

  return (
    <div className="m3uFilesEditor flex flex-column col-12 flex-shrink-0 ">
      <div className="flex justify-content-between align-items-center mb-1">
        <span className="m-0 p-0 gap-1" style={{ color: 'var(--orange-color)' }}>
          List of Profiles
        </span>
        <div className="m-0 p-0 flex gap-1">
          <ProfileAddDialog />
        </div>
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
