import { GetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { GetEPGFilePreviewByIdRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGFilePreviewById = createAsyncThunk('cache/getGetEPGFilePreviewById', async (param: GetEPGFilePreviewByIdRequest, thunkAPI) => {
  try {
    console.log('Fetching GetEPGFilePreviewById');
    const response = await GetEPGFilePreviewById(param);
    console.log('Fetched GetEPGFilePreviewById ',response?.length);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

