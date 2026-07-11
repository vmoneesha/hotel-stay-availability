import { DocumentType } from '../models/hotel.models';

const internationalDestinations = new Set(['London', 'Dubai', 'Singapore']);

export function requiredDocumentFor(destination: string): DocumentType {
  return internationalDestinations.has(destination) ? 'Passport' : 'NationalId';
}

export function documentLabel(documentType: DocumentType): string {
  return documentType === 'Passport' ? 'Passport' : 'National ID';
}