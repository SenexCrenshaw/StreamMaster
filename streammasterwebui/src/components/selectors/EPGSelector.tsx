import {
  ProgrammeNameDto,
  useProgrammesGetPagedProgrammeNameSelectionsQuery,
  useProgrammesGetProgrammsSimpleQueryQuery,
} from '@/lib/iptvApi'
import { GetProgrammeFromDisplayName } from '@/lib/smAPI/Programmes/ProgrammesGetAPI'
import React, { useCallback } from 'react'
import BaseSelector, { type BaseSelectorProps } from './BaseSelector'

type EPGSelectorProps = BaseSelectorProps<ProgrammeNameDto> & {
  enableEditMode?: boolean
}

const EPGSelector: React.FC<Partial<EPGSelectorProps>> = ({
  enableEditMode = true,
  onChange,
  ...restProps
}) => {
  const selectedTemplate = (option: ProgrammeNameDto) => {
    return <div>{option?.displayName}</div>
  }

  const handleOnChange = useCallback(
    (event: string) => {
      if (event && onChange) {
        onChange(event)
      }
    },
    [onChange],
  )

  const itemTemplate = (option: ProgrammeNameDto): JSX.Element => {
    return <div>{option?.displayName}</div>
  }

  if (!enableEditMode) {
    return (
      <div className="flex h-full justify-content-center align-items-center p-0 m-0">
        {restProps.value ?? 'Dummy'}
      </div>
    )
  }

  return (
    <BaseSelector
      {...restProps}
      editable
      itemSize={32}
      itemTemplate={itemTemplate}
      onChange={handleOnChange}
      optionLabel="displayName"
      optionValue="channel"
      queryFilter={useProgrammesGetPagedProgrammeNameSelectionsQuery}
      queryHook={useProgrammesGetProgrammsSimpleQueryQuery}
      querySelectedItem={GetProgrammeFromDisplayName}
      selectName="EPG"
      selectedTemplate={selectedTemplate}
    />
  )
}

EPGSelector.displayName = 'EPGSelector'
export default React.memo(EPGSelector)
