import { FieldData, PagedResponse } from '@lib/apiDefs';

export const updatePagedResponseFieldInData = (pagedResponse: PagedResponse<any> | undefined, fieldData: FieldData): PagedResponse<any> | undefined => {
  if (!pagedResponse) return undefined;

  console.log(pagedResponse);

  const updatedPagedResponse = {
    ...pagedResponse,
    data: pagedResponse.data.map((dto) => {
      const id = dto.id.toString();
      if (id === fieldData.id) {
        var test = {
          ...dto,
          [fieldData.field]: fieldData.value
        };
        return test;
      }
      return dto;
    })
  };

  return updatedPagedResponse;
};
