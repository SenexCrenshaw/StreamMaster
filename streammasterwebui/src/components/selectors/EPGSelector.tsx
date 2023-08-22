
import {
  type ProgrammeNameDto,
  useProgrammesGetProgrammsSimpleQueryQuery
} from '../../store/iptvApi';
import {
  useProgrammesGetProgrammeNameSelectionsQuery
} from '../../store/iptvApi';
import BaseSelector, { type BaseSelectorProps } from './BaseSelector';
import { type GetApiArg, type SimpleQueryApiArg } from '../../common/common';
import { GetProgrammeFromDisplayName } from '../../store/signlar_functions';
import React, { useState, useCallback } from 'react';

type EPGSelectorProps = BaseSelectorProps<ProgrammeNameDto> & {
  enableEditMode?: boolean;
};

const EPGSelector: React.FC<Partial<EPGSelectorProps>> = ({
  enableEditMode = true,
  onChange,
  ...restProps
}) => {

  const [paging, setPaging] = useState<SimpleQueryApiArg>({ first: 0, last: 40 });
  const [filter, setFilter] = useState<GetApiArg>({ pageSize: 40 });

  const data = useProgrammesGetProgrammeNameSelectionsQuery(paging);
  const filteredProgrammeData = useProgrammesGetProgrammsSimpleQueryQuery(filter);

  const selectedTemplate = (option: ProgrammeNameDto) => {
    return (
      <div>
        {option?.displayName}
      </div>
    );
  };

  const handleOnChange = useCallback((event: string) => {
    if (event && onChange) {
      onChange(event);
    }
  }, [onChange]);

  const itemTemplate = (option: ProgrammeNameDto): JSX.Element => {

    return (
      <div>
        {option?.displayName}
      </div>
    );
  }


  if (!enableEditMode) {
    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {restProps.value ?? 'Dummy'}
      </div>
    );
  }

  return (
    <BaseSelector
      {...restProps}
      data={data.data?.data ?? []}
      fetch={GetProgrammeFromDisplayName}
      filteredData={filteredProgrammeData.data ?? []}
      itemSize={32}
      itemTemplate={itemTemplate}
      onChange={handleOnChange}
      onFilter={setFilter}
      onPaging={setPaging}
      optionLabel="displayName"
      optionValue="channel"
      selectName='EPG'
      selectedTemplate={selectedTemplate}
    />
  );
};

EPGSelector.displayName = 'EPGSelector';


export default React.memo(EPGSelector);
