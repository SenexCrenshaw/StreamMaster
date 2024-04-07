import { FieldData } from '@lib/smAPI/smapiTypes';

export const updateFieldInData = (response: any | undefined, fieldData: FieldData): any | undefined => {
  if (!response) return undefined;

  const updatedResponse = response.map((dto: any) => {
    const id = dto.id.toString();
    if (id === fieldData.Id) {
      var test = {
        ...dto,
        [fieldData.Field]: fieldData.Value
      };
      return test;
    }
    return dto;
  });

  return updatedResponse;
};
