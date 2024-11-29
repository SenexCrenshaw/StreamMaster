import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { RemoveCustomLogo } from '@lib/smAPI/Logos/LogosCommands';
import useGetCustomLogos from '@lib/smAPI/Logos/useGetCustomLogos';
import { CustomLogoDto, RemoveCustomLogoRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo } from 'react';
import { getIconUrl } from './iconUtil';

export function CustomLogosDataSelector(): React.ReactElement {
  const { data } = useGetCustomLogos();
  const popupRef = React.useRef<SMPopUpRef>(null);
  Logger.debug('CustomLogosDataSelector', { data });

  const ReturnToParent = useCallback((didUpload?: boolean) => {
    popupRef.current?.hide();
  }, []);

  const imgTemplate = (rowData: any): React.ReactElement => {
    let url = getIconUrl(rowData.Value);

    return (
      <div className="sm-center-stuff">
        <img alt={rowData.Name} src={url} style={{ width: '32px' }} />
      </div>
    );
  };

  const onDelete = useCallback(
    (customLogoDto: CustomLogoDto) => {
      if (!customLogoDto.Source) {
        return;
      }
      const request = { Source: customLogoDto.Source } as RemoveCustomLogoRequest;
      RemoveCustomLogo(request)
        .then(() => {
          ReturnToParent(true);
        })
        .catch(() => {
          ReturnToParent(false);
        });
    },
    [ReturnToParent]
  );

  const actionTemplate = useCallback(
    (customLogoDto: CustomLogoDto) => {
      return (
        <div className="flex justify-content-end align-items-center" style={{ paddingRight: '0.1rem' }}>
          <SMPopUp
            buttonClassName="icon-red"
            icon="pi-times"
            info=""
            label=""
            modal
            noFullScreen
            onOkClick={() => onDelete(customLogoDto)}
            placement="bottom-end"
            ref={popupRef}
            title="Delete Custom Logo"
            zIndex={11}
          />
        </div>
      );
    },
    [onDelete]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: imgTemplate,
        field: 'Value',
        header: '',
        width: 2
      },
      {
        field: 'Name',
        filter: true,
        header: 'Name',
        sortable: true
      },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: 4 }
    ],
    [actionTemplate]
  );

  return (
    <SMDataTable
      dataSource={data?.filter((predicate) => predicate.IsReadOnly !== true)}
      noSourceHeader
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Custom Logos"
      enableExport={false}
      enablePaginator={(data?.length ?? 0) > 4}
      dataKey="Source"
      id="epgfilesdataselector"
    />
  );
}
