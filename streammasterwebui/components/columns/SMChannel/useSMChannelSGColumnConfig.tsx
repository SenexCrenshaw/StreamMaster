import EPGSGEditor from '@components/epg/EPGSGEditor';
import { SMTriSelectShowSG } from '@components/sm/SMTriSelectShowSG';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { AdditionalFilterProperties } from '@lib/common/common';
import { useQueryAdditionalFilters } from '@lib/redux/hooks/queryAdditionalFilters';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { ReactNode, useCallback, useEffect, useState } from 'react';

export const useSMChannelSGColumnConfig = (dataKey: string, id: string) => {
  const [previousSGID, setPreviousSGID] = useState<number>(0);
  const { queryAdditionalFilters, setQueryAdditionalFilters } = useQueryAdditionalFilters(id);

  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const { showHidden, setShowHidden } = useShowHidden(dataKey); //+ selectedStreamGroup?.Id ?? '0');

  const bodyTemplate = useCallback((bodyData: SMChannelDto) => {
    return <EPGSGEditor smChannel={bodyData} />;
  }, []);

  const updateFilters = useCallback(
    (hidden: boolean | null) => {
      if (hidden === true) {
        if (
          selectedStreamGroup &&
          (queryAdditionalFilters?.field !== 'sg' || queryAdditionalFilters.matchMode !== 'inSG' || queryAdditionalFilters.values !== selectedStreamGroup.Id)
        ) {
          const newFilter = { field: 'sg', matchMode: 'inSG', values: selectedStreamGroup.Id } as AdditionalFilterProperties;
          setQueryAdditionalFilters(newFilter);
        }
        return;
      }
      if (hidden === false) {
        if (
          selectedStreamGroup &&
          (queryAdditionalFilters?.field !== 'sg' || queryAdditionalFilters.matchMode !== 'notInSG' || queryAdditionalFilters.values !== selectedStreamGroup.Id)
        ) {
          const newFilter = { field: 'sg', matchMode: 'notInSG', values: selectedStreamGroup.Id } as AdditionalFilterProperties;
          setQueryAdditionalFilters(newFilter);
        }
        return;
      }

      setQueryAdditionalFilters(undefined);
    },
    [queryAdditionalFilters, selectedStreamGroup, setQueryAdditionalFilters]
  );

  useEffect(() => {
    if (!selectedStreamGroup) {
      setQueryAdditionalFilters(undefined);
      return;
    }

    if (previousSGID === 0) {
      setPreviousSGID(selectedStreamGroup.Id);
      setQueryAdditionalFilters(undefined);
      return;
    }

    if (selectedStreamGroup.Id !== previousSGID) {
      setPreviousSGID(selectedStreamGroup.Id);
      if (selectedStreamGroup.Id > 1) {
        updateFilters(showHidden);
      } else {
        setQueryAdditionalFilters(undefined);
        setShowHidden(null);
      }

      return;
    }
  }, [previousSGID, selectedStreamGroup, setQueryAdditionalFilters, setShowHidden, showHidden, updateFilters]);

  const filterTemplate = useCallback(
    (options: ColumnFilterElementTemplateOptions): ReactNode => {
      return (
        <SMTriSelectShowSG
          dataKey={dataKey}
          onChange={(e) => {
            updateFilters(e);
          }}
        />
      );
    },
    [dataKey, updateFilters]
  );

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'sg',
    fieldType: 'sg',
    filter: false,
    filterElement: filterTemplate,
    header: '',
    maxWidth: '2rem',
    minWidth: '2rem',
    sortable: false,
    width: '2rem'
  };

  return columnConfig;
};
